import csv
import os
from google.cloud import texttospeech

# USAGE: `run_tts.py`
# Place an API key for Google Text-to-Speech AI in `.tts-key.json`
# Export dialogue to be generated to `dialogue.csv`
# All missing lines will be generated automatically
# NOTE: API keys and dialogue CSVs are both gitignored, API keys for being
# sensitive information, and the CSV as it is generated

# I'm using Google cloud for this as, even though 11labs is better quality,
# it is also much more expensive.

DIALOGUE_FILE = "dialogue.csv"
DIALOGUE_DIRECTORY = "dialogue/"
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = ".tts-key.json"

client = texttospeech.TextToSpeechClient()
voice = texttospeech.VoiceSelectionParams(
    language_code = "en-GB",
    name = "en-GB-Chirp3-HD-Enceladus",
    ssml_gender = texttospeech.SsmlVoiceGender.MALE
)
audio_config = texttospeech.AudioConfig(
    audio_encoding=texttospeech.AudioEncoding.MP3)

def synthesise(text: str, file: str):
    input = texttospeech.SynthesisInput(text=text)
    response = client.synthesize_speech(input=input, voice=voice, audio_config=audio_config)

    with open(file, 'wb') as out:
        out.write(response.audio_content)
        print(f"Written TTS to {file}")

def main():
    print("Running synth")
    if not os.path.exists(DIALOGUE_DIRECTORY):
        os.mkdir(DIALOGUE_DIRECTORY)

    with open(DIALOGUE_FILE, newline='') as csvfile:
        reader = csv.DictReader(csvfile)
        for row in reader:
            if row["Voiced"] == "True": continue

            text = row["Text"]
            id = row["ID"]
            synthesise(text, f"{DIALOGUE_DIRECTORY}/{id}.mp3")

if __name__ == "__main__":
    main()
