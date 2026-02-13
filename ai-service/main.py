from fastapi import FastAPI, File, UploadFile, HTTPException, BackgroundTasks
import os
# import uuid
# import shutil
import numpy as np
import cv2
import logging
from contextlib import asynccontextmanager
from deepface import DeepFace
from cv2 import threshold

# Setup logging for observability, monitoring and feedback
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("biometric_ai_service")


# Scalability and performance optimization
@asynccontextmanager
async def lifespan(app: FastAPI):
    logger.info("Starting up: Loading AI models into memory...")
    try:
        # Preloading the model prevents the first user from experiencing delay
        DeepFace.build_model("Facenet512")
        logger.info("Models loaded successfully.")
    except Exception as e:
        logger.error(f"Critical error loading models: {e}")
    yield
    logger.info("Shutting down: Cleaning up resources...")
    

app = FastAPI(title="Biometric AI Service", version="1.0", lifespan=lifespan)


# HELPER FUNCTIONS
def cleanup_temp_file(file_path: str):
    """PILLAR 5: SECURITY - Ensure raw images are deleted immediately."""
    if os.path.exists(file_path):
        os.remove(file_path)
        logger.info(f"Temporary file {file_path} deleted.")


def is_image_blurry(image, threshold=20.0):
    # Higher Value = Sharper Image, Lower Value = Blurry Image
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    variance = cv2.Laplacian(gray, cv2.CV_64F).var()
    logger.info(f"Image sharpness score: {variance:.2f}")
    return variance < threshold, variance

  
# The Core Feature --------
@app.post("/extract-embedding", status_code=200)
async def extract_embedding(background_tasks: BackgroundTasks, file: UploadFile=File(...)):
    # Receives an image file, extracts a 512-d facial vector, and returns it as JSON
    # Performance - validate file type before processing to avoid unnecessary overhead
    if not file.content_type.startswith("image/"):
        raise HTTPException(status_code=400, detail="Invalid file type. Please upload an image.")
    
    # temp_filename = f"prod_temp_{uuid.uuid4()}.jpg"
    
    try:
        # Save file to a temporary location for processing
        # with open(temp_filename, "wb") as buffer:
        #     shutil.copyfileobj(file.file, buffer)
        
        contents = await file.read()
        # Convert to OPENCV format
        nparr = np.frombuffer(contents, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        
        if img is None:
            raise ValueError("Could not decode the image. Please ensure it's a valid image file.")
        
        # Privacy - Ensure the image is not blurry to avoid processing low-quality photo
        is_blurry, score = is_image_blurry(img)
        if is_blurry:
            raise HTTPException(
                status_code=422,
                detail=f"The uploaded image is too blurry (sharpness score: {score:.2f}). Please upload a clearer photo of a face."
            )
            
        # AI LOGIC - Extract facial embedding using DeepFace
        # Enforce_detection=True ensures we don't return vectors for non-faces
        results = DeepFace.represent(
            img_path=img,
            model_name="Facenet512",
            enforce_detection=True,
            detector_backend="opencv"
        )
        
        # Performance - Check for multiple faces and reject to avoid incorrect embeddings
        face_count = len(results)
        if face_count > 1:
            logger.warning(f"Rejected: {face_count} faces detected.")
            raise HTTPException(
                status_code=422,
                detail=f"Multiple faces detected ({face_count}). Please upload an image with only one clear face."
            )
        
        analysis = DeepFace.analyze(
            img_path=img,
            actions=['age', 'gender', 'race', 'emotion'],
            enforce_detection=True,
            detector_backend="opencv"
        )
        
        # Privacy - Clean up the raw photo immediately after AI processing
        # background_tasks.add_task(cleanup_temp_file, img)  # Registering the function to be called later 
        
        return {
            "success": True,
            "embedding": results[0]["embedding"],  # Return the 512-d vector
            "face_confidence": results[0]["face_confidence"],
            "attributes": {
                "age": analysis[0]["age"],
                "dominant_gender": analysis[0]["dominant_gender"],
                "dominant_race": analysis[0]["dominant_race"],
                "dominant_emotion": analysis[0]["dominant_emotion"]
            }
        }
    
    except HTTPException as http_exc:
        logger.warning(f"HTTP Exception: {http_exc.detail}")
        raise http_exc  # Re-raise to be handled by FastAPI
            
    except ValueError:
        # This error is raised when no face is detected in the image
        # background_tasks.add_task(cleanup_temp_file, img)  # Ensure cleanup even on failure
        logger.warning("No face detected in the uploaded image.")
        raise HTTPException(status_code=422, detail="No face detected in the image. Please upload a clear photo of a face.")
    
    except Exception as e:
        logger.error(f"Internal Error: {e}")
        # background_tasks.add_task(cleanup_temp_file, img)  # Ensure cleanup even on failure
        raise HTTPException(status_code=500, detail="An error occurred while processing the image. Please try again.")


@app.get("/health")
def home():
    # Health check for load balancers to ensure stability and uptime
    return {"status": "healthy", "message": "Biometric AI Service is up and running!", "model": "Facenet512"}
