
from transformers import GPT2LMHeadModel, GPT2Tokenizer, pipeline
from sentence_transformers import SentenceTransformer, util
import torch

# Sentiment analysis 

sentence = """aa%20fuck%20aaa%20shit%e2%80%8b"""  # Example sentence
sentiment_pipeline = pipeline("sentiment-analysis", device=0)  # Load sentiment analysis pipeline on GPU
model = GPT2LMHeadModel.from_pretrained('gpt2')
tokenizer = GPT2Tokenizer.from_pretrained('gpt2')
sent_model = SentenceTransformer('all-MiniLM-L6-v2')

MIN_PERPLEXITY = 10
MAX_PERPLEXITY = 100

def sentiment_score(sentence):
    
    result = sentiment_pipeline(sentence)
    score = result[0]['score']
    label = result[0]['label']
    print(score)
    print(label)
    if label == 'NEGATIVE':
        score = 1 - score
    else:
        score += 1
    
    return score / 2

print(sentiment_score(sentence))  # Output includes label (positive/negative) and confidence score

def calculate_perplexity(sentence):
    inputs = tokenizer(sentence, return_tensors="pt")
    with torch.no_grad():
        loss = model(**inputs, labels=inputs["input_ids"]).loss
    perplexity = torch.exp(loss)
    return perplexity.item()

def fluency_score(sentence):
    # Calculate perplexity
    perplexity = calculate_perplexity(sentence)
    
    # Normalize the perplexity to a 0-1 fluency score
    if perplexity <= MIN_PERPLEXITY:
        return 1.0  # Maximum fluency
    elif perplexity >= MAX_PERPLEXITY:
        return 0.0  # Minimum fluency
    else:
        # Apply min-max normalization formula
        return 1 - (perplexity - MIN_PERPLEXITY) / (MAX_PERPLEXITY - MIN_PERPLEXITY)
    
def calculate_relevance(response, question):
    # Generate embeddings for the question and response
    question_embedding = sent_model.encode(question, convert_to_tensor=True)
    response_embedding = sent_model.encode(response, convert_to_tensor=True)
    
    # Calculate cosine similarity
    similarity_score = util.cos_sim(response_embedding, question_embedding).item()
    return round(similarity_score, 2)

# Example usage
question = "What is your greatest strength?"
response = "I believe my greatest strength is my ability to work well in a team."
score = calculate_relevance(response, question)
print(score)  # Output relevance score between 0 and 1

score = fluency_score(sentence)

print(str(score))  # Output fluency score between 0 and 1
