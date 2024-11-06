
from transformers import GPT2LMHeadModel, GPT2Tokenizer, pipeline
import torch

# Sentiment analysis 

sentence = """yeah you know yeah haha."""  # Example sentence
sentiment_pipeline = pipeline("sentiment-analysis", device=0)  # Load sentiment analysis pipeline on GPU
model = GPT2LMHeadModel.from_pretrained('gpt2')
tokenizer = GPT2Tokenizer.from_pretrained('gpt2')
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

score = fluency_score(sentence)

print(score)  # Output fluency score between 0 and 1
