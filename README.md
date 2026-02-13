# 🛡️ Biometric Identity System

> A biometric authentication platform combining the speed of **C# .NET 9** with the AI precision of **Python FastAPI**.

---

## 📖 Overview

The application is a privacy-first biometric identity system. By decoupling the Identity Orchestrator (C#) from the AI Logic (Python), the system achieves high scalability and security. 

It utilizes mathematical facial embeddings rather than raw image storage, ensuring user privacy and GDPR compliance.

---

## System Architecture

| Layer | Technology| Role|
|---|---|---|
| **UI** | React Native | Captures high-quality camera frames and manages the mobile UI/UX. |
| **Identity Gateway** | C# (ASP.NET Core 10) | Manages JWT, User Profiles, and orchestrates the AI Service. |
| **AI Micro-Service** | Python (FastAPI) | Performs face detection, embedding extraction, and quality gates. |
| **Database** | PostgreSQL + pgvector | Stores high-dimensional 512-d embeddings for similarity search. |

## The AI Micro-Service

- **Stateless Processing**: Operates entirely in RAM; images are never written to disk, ensuring maximum security.
- **Facenet512 Integration**: Generates robust 512-dimensional facial vectors for sub-millimetric accuracy.
- **Quality Gates**: Automated Laplacian variance checks to reject blurry images before they enter the system.
- **Identity Integrity**: Enforces a "One-Face-Only" rule to prevent enrollment collisions.

---

# The Workflow

## 1. Enrollment (Registration)
- **React Native**: User captures a selfie. The app transmits the base64 or multipart image to the C# Gateway.
- **C# Backend**: Validates the session and forwards the payload to the Python AI Service.
- **Python AI**: Converts the face into a mathematical embedding (vector) and performs a quality check.
- **C# Backend**: Persists the resulting vector into PostgreSQL using pgvector linked to the User ID.

## 2. Authentication (Login)
- **React Native**: User scans their face.
- **C# Gateway**: Forwards the scan to the Python AI.
- **Database**: C# executes a Cosine Similarity query via pgvector to compare the new scan against the stored embedding.
- **Result**: If the distance is within the security threshold, a JWT is issued.

---

# Running Locally

## 1. Prerequisites
- .NET 9.0 SDK
- Node.js
- Python 3.10+
- PostgreSQL with pgvector extension installed.

## 2. Setup AI Service (Python)
```
cd ai-service
pip install -r requirements.txt
uvicorn main:app --port 8000
```

## 🙋‍♂️ Contributor & Maintainer
### Mololuwa Segilola

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mololuwa-segilola/)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/molodev.gg/)
[![Email](https://img.shields.io/badge/Email-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:mololuwa.segilola10@gmail.com)
[![Medium](https://img.shields.io/badge/Medium-12100E?style=for-the-badge&logo=medium&logoColor=white)](https://medium.com/@segilolamololuwa)

<!--
- [LinkedIn](https://www.linkedin.com/in/mololuwa-segilola/)
- [Instagram](https://www.instagram.com/molodev.gg/)
- [Portfolio](https://segilolamololuwa.vercel.app/)
-->
  
💬 Always happy to talk about this project or other full-stack engineering challenges.

