import json
import csv
import os
from google.cloud import texttospeech

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

synthesise("Hello there world", "test.mp3")
