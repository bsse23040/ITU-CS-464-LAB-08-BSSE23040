"""
Makes every sound and music track for Lab 08 (Comet Dash).

Nothing is recorded or downloaded: each sound is built from sine waves, a little noise and an
envelope (how loud it is over time). Run it from the project folder:

    python Docs/make_audio.py

It writes the .wav files into Assets/Audio/.
"""
import os
import wave
import numpy as np

SR = 44100
OUT = "Assets/Audio/"
rng = np.random.default_rng(8)          # the same noise every run, so the files do not change


def write(name, x, peak):
    x = x / max(1e-9, np.max(np.abs(x))) * peak
    data = (np.clip(x, -1, 1) * 32767).astype("<i2")
    with wave.open(OUT + name + ".wav", "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SR)
        w.writeframes(data.tobytes())
    print(f"{name:14s}{len(x) / SR:6.2f} s")


def time(dur):
    return np.arange(int(dur * SR)) / SR


def tone(freq, dur, decay=8.0, attack=0.004, harmonics=(1.0, 0.25, 0.08)):
    t = time(dur)
    y = sum(a * np.sin(2 * np.pi * freq * (k + 1) * t) for k, a in enumerate(harmonics))
    return y * np.minimum(1.0, t / attack) * np.exp(-decay * t)


def sweep(f0, f1, dur, decay=6.0):
    """A tone whose pitch slides from f0 to f1."""
    t = time(dur)
    freq = f0 * (f1 / f0) ** (t / dur)
    return np.sin(2 * np.pi * np.cumsum(freq) / SR) * np.minimum(1.0, t / 0.005) * np.exp(-decay * t)


def noise(dur, width):
    n = int(dur * SR)
    return np.convolve(rng.standard_normal(n), np.ones(width) / width, mode="same")


def place(total, parts):
    out = np.zeros(int(total * SR))
    for start, x in parts:
        i = int(start * SR)
        out[i:i + len(x)] += x[:max(0, len(out) - i)]
    return out


# ------------------------------------------------------------------ interface sounds
def ui():
    write("ui_hover", tone(1900, 0.05, 90, harmonics=(1.0,)), 0.35)
    write("ui_click", tone(980, 0.09, 55) + 0.25 * noise(0.09, 3) * np.exp(-260 * time(0.09)), 0.6)
    write("ui_back", place(0.2, [(0.0, tone(700, 0.1, 35)), (0.06, tone(470, 0.13, 30))]), 0.55)
    write("ui_open", sweep(300, 900, 0.22, 7) + 0.18 * noise(0.22, 14) * np.sin(np.pi * time(0.22) / 0.22), 0.32)
    write("ui_close", sweep(800, 260, 0.2, 8) + 0.15 * noise(0.2, 18) * np.sin(np.pi * time(0.2) / 0.2), 0.3)
    write("ui_pause", place(0.4, [(0.0, tone(660, 0.2, 14)), (0.11, tone(440, 0.28, 11))]), 0.55)
    write("ui_resume", place(0.4, [(0.0, tone(440, 0.2, 14)), (0.11, tone(660, 0.28, 11))]), 0.55)
    write("ui_success", place(0.55, [(0.0, tone(784, 0.2, 12)), (0.09, tone(1047, 0.2, 12)), (0.18, tone(1568, 0.36, 8))]), 0.55)
    t = time(0.3)
    buzz = np.sign(np.sin(2 * np.pi * 150 * t)) * 0.4 + np.sin(2 * np.pi * 157 * t)
    gate = ((t < 0.11) | (t > 0.15)).astype(float)
    write("ui_error", np.convolve(buzz * gate, np.ones(24) / 24, mode="same") * np.exp(-4 * t), 0.5)


# ------------------------------------------------------------------ gameplay sounds
def game():
    write("sfx_coin", place(0.38, [(0.0, tone(1318.5, 0.3, 11)), (0.06, tone(1975.5, 0.3, 11))]), 0.6)
    t = time(0.45)
    thump = np.sin(2 * np.pi * np.cumsum(150 * np.exp(-9 * t) + 45) / SR) * np.exp(-8 * t)
    write("sfx_hit", noise(0.45, 11) * np.exp(-15 * t) * 0.9 + thump * 1.1, 0.85)
    write("sfx_heal", place(0.8, [(0.1 * i, tone(f, 0.5, 6, attack=0.02)) for i, f in enumerate((523.25, 659.25, 783.99, 1046.5))]), 0.55)
    t = time(0.7)
    whoosh = noise(0.7, 5) * (t / 0.7) * np.exp(-3.5 * t) * 2.5
    write("sfx_boost", whoosh + 0.6 * sweep(180, 1100, 0.7, 2.5), 0.65)
    t = time(1.1)
    boom = np.sin(2 * np.pi * np.cumsum(120 * np.exp(-4 * t) + 30) / SR) * np.exp(-4 * t)
    write("sfx_death", noise(1.1, 30) * np.exp(-5 * t) * 2.2 + boom, 0.9)
    notes = [523.25, 659.25, 783.99, 1046.5]
    parts = [(0.12 * i, tone(f, 1.3, 2.8)) for i, f in enumerate(notes)] + [(0.5, tone(f, 1.0, 3.2) * 0.5) for f in notes[:3]]
    write("sfx_win", place(1.8, parts), 0.75)
    parts = [(0.3 * i, tone(f, 1.0, 3.4, harmonics=(1.0, 0.5, 0.28, 0.12))) for i, f in enumerate((392.0, 311.13, 261.63, 196.0))]
    write("sfx_lose", place(1.7, parts), 0.75)


# ------------------------------------------------------------------ music
def hz(m):
    return 440.0 * 2 ** ((m - 69) / 12)


def add(out, start, x, gain):
    """Mixes x into out at start seconds. What runs past the end wraps to the start, so loops have no seam."""
    i = int(start * SR) % len(out)
    first = min(len(x), len(out) - i)
    out[i:i + first] += gain * x[:first]
    if first < len(x):
        out[:len(x) - first] += gain * x[first:]


def loop(name, bpm, chords, arp, bass_every, kick_beats, hat, peak):
    beat = 60 / bpm
    bar = 4 * beat
    out = np.zeros(int(len(chords) * 2 * bar * SR))
    for c, (notes, root) in enumerate(chords):
        for b in range(2):
            start = (c * 2 + b) * bar
            for m in notes:                                       # a soft pad that holds the chord
                t = time(bar)
                env = np.minimum(1, t / 0.5) * np.minimum(1, (bar - t) / 0.4)
                add(out, start, (np.sin(2 * np.pi * hz(m) * t) + 0.2 * np.sin(4 * np.pi * hz(m) * t)) * env, 0.10)
            steps = len(arp)
            for k, idx in enumerate(arp):                         # a plucked pattern over the chord
                if idx is None:
                    continue
                add(out, start + k * bar / steps, tone(hz(notes[idx % 3] + 12 + 12 * (idx // 3)), 0.6, 6, harmonics=(1.0, 0.18)), 0.11)
            for k in range(0, 4, bass_every):
                add(out, start + k * beat, tone(hz(root), 0.55, 4, harmonics=(1.0, 0.15)), 0.28)
            for k in kick_beats:
                t = time(0.2)
                add(out, start + k * beat, np.sin(2 * np.pi * np.cumsum(110 * np.exp(-25 * t) + 42) / SR) * np.exp(-14 * t), 0.30)
            if hat:
                for k in range(hat):
                    if k % 2 == 1:
                        t = time(0.05)
                        add(out, start + k * bar / hat, np.diff(noise(0.05 + 1 / SR, 2))[:len(t)] * np.exp(-70 * t), 0.45)
    write(name, out, peak)


def music():
    minor = [([57, 60, 64], 45), ([53, 57, 60], 41), ([48, 52, 55], 36), ([55, 59, 62], 43)]
    # menu: slow and open, no drums
    loop("music_menu", 84, minor, [0, None, 1, None, 2, None, 3, None], 2, [], 0, 0.5)
    # gameplay: faster, with a kick on every beat and off-beat hats
    run = [([52, 55, 59], 40), ([48, 52, 55], 36), ([50, 53, 57], 38), ([47, 50, 54], 35)]
    loop("music_game", 132, run, [0, 1, 2, 3, 2, 1, 0, 2], 1, [0, 1, 2, 3], 8, 0.6)
    # results: warm, major
    major = [([48, 52, 55], 36), ([53, 57, 60], 41)]
    loop("music_results", 96, major, [0, 1, 2, 3, 4, 3, 2, 1], 2, [0, 2], 0, 0.5)


if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    ui()
    game()
    music()
