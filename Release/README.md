# Amnesia Mead

**Amnesia Mead** is a lightweight BepInEx utility mod for *Valheim* that resets character cheat counters & flags.

Odin is watching... but after a mug of Amnesia Mead, he forgets everything.

---

## Features

* **Wipes the Cheats Counter:** Resets the `Cheats` stat counter back to `0` in your character profile (`.fch`).
* **Clears Command History:** Flushes recorded command usages (`god`, `spawn`, `raiseskill`, etc.) from your profile data.
* **Removes Spawned Item Flags:** Clears debug items (e.g., `Cheat sledge`) from your pickup history.
* **Native Console Command:** Integrates cleanly into Valheim’s `F5` terminal via a single command.

---

## Installation

### Prerequisites
* [BepInEx Pack for Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) installed.

### Manual Installation
1. Download the latest `AmnesiaMead.dll` release.
2. Place `AmnesiaMead.dll` into your Valheim installation folder under `BepInEx/plugins/`.
3. Launch Valheim.

---

## Usage

> **Note:** Take a manual backup of your character file in case this corrupts your character data.

1. Load into any world with the character you wish to clean.
2. Open the console (**F5**).
3. Type the command:
   `amnesia-cheats`

---

## License

Distributed under the MIT License. See `LICENSE` for more information.