from flask import Flask
from openai import OpenAI

app = Flask(__name__)


client = OpenAI(
  organization='org-bkTtf8lP8uYZyMdx6PZEvGIK',
  project="proj_y93Oq35l2UyIsxnQerJeH3ee"
)

@app.route("/")
def hello_world():
    """
    This function returns a simple HTML string.
    """

    completion = client.chat.completions.create(
    model="gpt-4o-mini",
    messages=[
        {"role": "system", "content": "You are a helpful assistant."},
        {
            "role": "user",
            "content": "Write a haiku about recursion in programming."
        }
    ])

    return completion.choices[0].message.content
