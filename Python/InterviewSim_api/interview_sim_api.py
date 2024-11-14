from flask import Flask, request
from openai import OpenAI
from transformers import GPT2LMHeadModel, GPT2Tokenizer, pipeline
from sentence_transformers import SentenceTransformer, util
import torch

app = Flask(__name__)


client = OpenAI(
  organization='org-bkTtf8lP8uYZyMdx6PZEvGIK',
  project="proj_y93Oq35l2UyIsxnQerJeH3ee"
)

sentiment_pipeline = pipeline("sentiment-analysis", device=0)  # Load sentiment analysis pipeline on GPU
model = GPT2LMHeadModel.from_pretrained('gpt2')
tokenizer = GPT2Tokenizer.from_pretrained('gpt2')
sent_model = SentenceTransformer('all-MiniLM-L6-v2')

MIN_PERPLEXITY = 10
MAX_PERPLEXITY = 100

class InterviewSimData:
    def __init__(self):
        self.response = str("")

    def set_response(self, response: str):
        self.response = response

    def get_response(self):
        return self.response
    
interview_sim_data = InterviewSimData()

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

@app.route("/send_interviewer_text", methods=["GET", "POST"])
def hello_world():
    """
    This function receives the user's text input and sends it to the OpenAI API to generate a response. 
    """

    content = request.get_data(as_text=True)
    print(content)
    completion = client.chat.completions.create(
    model="gpt-4o-mini",
    messages=[
        {"role": "system", "content": "You are an HR expert specializing in conducting professional job interviews."},
        {"role": "user", "content": content},
    ]
    )

    response = completion.choices[0].message.content
    interview_sim_data.set_response(response)

    return response

@app.route("/fluency_sentiment_score", methods=["GET", "POST"])
def fluency_sentiment_score():
    """
    This function receives the user's text input and calculates both the fluency and sentiment scores.
    """

    content = request.get_data(as_text=True)
    print(content)
    fluency = fluency_score(content)
    sentiment = sentiment_score(content)
    relevancy = calculate_relevance(content, interview_sim_data.get_response())

    return str(fluency) + "," + str(sentiment) + "," + str(relevancy)
