from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from fastembed import TextEmbedding, SparseTextEmbedding
from typing import List

app = FastAPI()

dense_model = None
sparse_model = None
        
@app.on_event("startup")
async def load_models():
    global dense_model, sparse_model
    print("Loading embedding models...")
    from concurrent.futures import ThreadPoolExecutor
    import asyncio

    loop = asyncio.get_event_loop()
    with ThreadPoolExecutor() as pool:
        dense_model, sparse_model = await asyncio.gather(
            loop.run_in_executor(pool, TextEmbedding, "sentence-transformers/all-MiniLM-L6-v2"),
            loop.run_in_executor(pool, SparseTextEmbedding, "Qdrant/bm42-all-minilm-l6-v2-attentions"),
        )
    print("Models loaded successesful")


class TextInput(BaseModel):
    texts: List[str]

@app.post("/embed/dense")
def embed_dense(input: TextInput):
    if dense_model is None:
        raise HTTPException(status_code = 503, detail = "Dense model not loaded")
    try:
        embeddings = list(dense_model.embed(input.texts))
        return {"embeddings": [embedding.tolist() for embedding in embeddings]}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/embed/sparse")
def embed_sparse(input: TextInput):
    if sparse_model is None:
        raise HTTPException(status_code = 503, detail = "Sparse model not loaded")
    try:
        embeddings = list(sparse_model.embed(input.texts))
        result = []
        for emb in embeddings:
            result.append({
                "indices": emb.indices.tolist(),
                "values": emb.values.tolist()
            })
        return {"embeddings": result}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
