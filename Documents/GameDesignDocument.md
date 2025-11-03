<h1 align="center">Echo City - Game Design Document</h1>
<div align="center">
    <table>
        <thead>
            <tr>
                <th>👤 Name</th>
                <th>🎯 Role</th>
                <th>✉️ Contact</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Giovanni Passuello</td>
                <td>Leader - Programmer</td>
                <td><a href="mailto:giovanni.passuello@mail.polimi.it">giovanni.passuello@mail.polimi.it</a></td>
            </tr>
             <tr>
                <td>Jie Shen</td>
                <td>Programmer</td>
                <td><a href="mailto:shenjie0203@outlook.com">shenjie0203@outlook.com</a></td>
            </tr>
            <tr>
                <td>Samuel Runmark Thunell</td>
                <td>Programmer</td>
                <td><a href="mailto:samuelthunell@hotmail.com">samuelthunell@hotmail.com</a></td>
            </tr>
            <tr>
                <td>Marco Moretta</td>
                <td>Programmer</td>
                <td><a href="mailto:omegamorello@gmail.com">omegamorello@gmail.com</a></td>
            </tr>
            <tr>
                <td>Lorenzo Ricci</td>
                <td>Programmer</td>
                <td><a href="mailto:lorenzo4.ricci@mail.polimi.it">lorenzo4.ricci@mail.polimi.it</a></td>
            </tr>
        </tbody>
    </table>
</div>

<p align="center">⚙️ Version: 0.2 &nbsp;•&nbsp; 🕒 Last modified: 2025-11-03</p>

## Table of Contents

- [0. Document Versioning](#0-document-versioning)
- [1. Overview](#1-overview)
  - [1.1. Genre - Gameplay](#11-genre---gameplay)
  - [1.2. Themes and Setting](#12-themes-and-setting)
  - [1.3. Core Mechanics Overview](#13-core-mechanics-overview)
  - [1.4 Project Scope](#14-project-scope)
- [2. Story](#2-story)
- [3. Characters](#3-characters)
- [4. Core Gameplay Mechanics](#4-core-gameplay-mechanics)
- [5. Level Design](#5-level-design)
  - [5.1. Noah's Lab - Tutorial Level](#51-noahs-lab---tutorial-level)
  - [5.2. The Market](#52-the-market)
  - [5.3. The Resonance Tower](#53-the-resonance-tower)
- [6. UI / HUD Design](#6-ui--hud-design)
- [7. Assets](#7-assets)
- [8. Soundtrack](#8-soundtrack)

## 0. Document Versioning
| **Version** | **Date**       | **Change Description**                  |
|-------------|------------------------------|-----------------------------------------|
| 0.1         | 2025-10-30     | Initial draft completed                 |
| 0.2         | 2025-11-03     | Added some general level design details in sections 5.1, 5.2, 5.3 and their subsections. |

## 1. Overview

### 1.1. Genre - Gameplay

| **Element** | **Description** |
|---|---|
| **Genre** | 3D, First-person, Exploration, Puzzle, Experimental |
| **Game Style** | Single-player |

### 1.2. Themes and Setting

| **Theme** | **Description** |
|---|---|
| **Narrative Theme** | Guilt and redemption, sacrifice, isolation. |
| **Systemic Theme** | Sound attracts the enemy and reveals the environment. |
| **Aesthetic Theme** | Darkness interrupted by chromatic sound pulses. |

### 1.3. Core Mechanics Overview


| **Mechanic** | **Characteristics** |
|---|---|
| **Echolocation** | Any sound emitted by the player or the environment allows the player to detect parts of the environment that would otherwise be dark. |
| **Sound Tools** | Each sound tool used has the side effect of attracting the monsters. |
| **Puzzles and Exploration** | The interaction of the two dynamics above, together with the narrative path of each level, leads the player to solve puzzles in order to remain in the shadows and pursue their objectives. |

### 1.4 Project Scope

The division of roles, as well as tasks and their deadlines, is managed via GitHub Projects within the project repository.

In addition to file sharing on GitHub, a shared OneDrive folder is used for large files (e.g., 3D assets, audio, etc.).

## 2. Story

In 2130, many scientists were involved in the Resonance Project. The planet’s air had become unbreathable due to pollution, and the goal of the project was to exploit a combination of ultrasound and infrasound to filter the air of its polluted component and channel the latter to specific places on Earth.

From the outset the experiments were successful; however, the scientists had not foreseen that the use of these frequencies would have a side effect on the life forms present in the areas where the pollutant particles were concentrated.
The fauna and flora of these zones changed radically, giving rise to creatures and plants never seen before. The effect on humans was even more unsettling, giving rise to deformed and aggressive mutants.

Little is known about the creatures born in that area: they are hostile, unbeatable in daylight, and ultra-sensitive to sounds, particularly those emitted by the Resonance Towers, enormous structures that continue to emit the frequencies necessary for the project’s operation.

The protagonist, Noah Ward, is one of the last scientists still alive.
He once belonged to the research team of the Resonance Project, personally contributing to the tuning of the waves that were supposed to save the planet.
Today he lives haunted by guilt for what he helped unleash: the destruction of the world as he knew it. His task is to recover the project’s data and remedy the catastrophe.

During his contribution to the project, Noah had heard rumors about a certain Doctor Kael, a scientist who opposed the use of those frequencies, having foreseen the catastrophic consequences.
Today, thanks to the information left by Kael, the world might perhaps return to normal thanks to the very towers that caused its ruin.

To survive the monsters, Noah developed a device that allows him to move through areas of complete darkness: glasses based on echolocation.
Every sound emitted is reflected by surrounding objects and captured by sensors on the frame, which process the information and translate it into a visual representation of the environment.

## 3. Characters

| Name        | Role         | Description                                                     | Motivations                                     |
|-------------|--------------|-----------------------------------------------------------------|-------------------------------------------------|
| Noah Ward   | Protagonist  | Former scientist of the Resonance Project, survivor of the catastrophe. | To atone for his role in the destruction of the world. |
| Doctor Kael | Mentor       | Scientist who opposed the Resonance Project.                    | To stop the project and its consequences.       |
| Creatures   | Antagonists  | Deformed creatures born from exposure to polluted air.          | To hunt and kill humans.                        |

## 4. Core Gameplay Mechanics

### 4.1. Echolocation System

Any sound, emitted by **player-operated sound tools** or by the **environment**, temporarily reveals otherwise dark areas through the echolocation device. Environmental sounds can be **recurring** or **occasional** and, like tools, both **attract creatures** and **reveal portions of the environment**.

**Sound properties** drive echolocation and risk:

- **Intensity (volume)** -> **detection range (distance)**
- **Duration (time)** -> **duration of the reveal**
- **Frequency (wavelength)** -> **reveal detail** and **range**.

Each property contributes, at different weights, to **enemy attraction**.

**Mapping (property -> echolocation parameter):**

| Sound Property | Echolocation Parameter |
|---|---|
| Intensity | Detection range (distance) |
| Duration  | Reveal duration |
| Frequency | Reveal detail **+** Detection range |

### 4.2. Sound Types and Effects

**Intensity -> echolocation/risk**:

| Intensity | Echolocation Effect      | Risk (Attraction) |
|---|---|---|
| Low   | Short-range reveal | Low   |
| Medium| Mid-range reveal   | Medium|
| High  | Long-range reveal  | High  |

*Note:* the contribution of intensity to attraction **decreases with distance** from the creature.

**Duration -> echolocation/risk**:

| Duration     | Echolocation Effect    | Risk (Attraction) |
|---|---|---|
| Transient    | Brief reveal           | Instant |
| Continuous   | Prolonged reveal       | Prolonged |

*Note:* duration controls how long the environment remains visible after emission.

**Frequency -> echolocation/risk**:

| Frequency | Echolocation Effect        | Risk (Attraction) |
|---|---|---|
| Low   | Coarse reveal           | Low    |
| Mid   | Near-intelligible reveal| Medium |
| High  | Sharp reveal            | High   |

*Note:* **high frequencies attract creatures more strongly.**

**Frequency -> range**:

| Frequency | Detection Range |
|---|---|
| Low   | Long-range  |
| Mid   | Mid-range   |
| High  | Short-range |

**Balancing note:** to simplify development and tuning, sounds are selected within **discrete frequency bands** (low/mid/high) and **discrete intensity levels** (low/medium/high).

### 4.3. Enemy AI Behavior

Creatures are **attracted by sound sources** according to intensity, duration, and frequency. Attraction from intensity **falls off with distance**; **higher frequencies** increase the chance of drawing enemies. If the player is **reached and killed**, the level **fails** (respawn at level start or last checkpoint).

### 4.4 Summary of Mechanics

## 5. Level Design

**Echo City** will consist of three levels:

1. **Noah’s Lab** – Initial Level / Tutorial
2. **The Market** – Development  
3. **The Resonance Tower** – Conclusion

### 5.1. Noah's Lab - Tutorial Level

#### 5.1.1 Level Overview

The setting is the underground laboratory where the protagonist lives. During this level, the player:

- is introduced to the story;
- tries the newly developed **echolocation** prototype;
- encounters the first enemies, testing the main gameplay dynamics.

The player is guided through a **simple puzzle** to become familiar with the controls, during which **it is not possible to die**; on-screen tips are shown on how to proceed.  

**Level ends:** when the guided puzzle is solved.

#### 5.1.2 Sound Tools

- Footsteps, low frequency sound that contributes to perception throughout the entire game
- Clapping, mid frequency which reveals more than the footsteps
- Whistles, high frequency sound that reveals the most details but attracts enemies the most

#### 5.1.3 Environmental Sound Sources

- Water Drops falling from a leaking pipe in the underground lab's corridor.
- Electronics producing constant humming, radios and lab equipment
- Falling objects

#### 5.1.4 Enemies

- Basic, dumb enemies to give the player a chance at the start

#### 5.1.5 Puzzles

The monsters are blocking the exit; the player must use sound to attract them and reach the exit.
First interaction with sound tools and environmental sounds.
Within this puzzle the player will also find the first continuous sound tool which will be useful in the next levels being able to reveal (in a non detailed way) the environment in a different way.

### 5.2. The Market

#### 5.2.1 Level Overview

The player reaches the downtown market area hoping to find information about **Doctor Kael** and trace the data needed to remedy the catastrophe.  
The **puzzles** are more **complex** and the **monsters** more **aggressive**.

**Level ends:** when Noah finds the **information** he is looking for.

#### 5.2.2 Sound Tools

- Bell, high frequency sound that reveals a large area but attracts enemies significantly

#### 5.2.3 Environmental Sound Sources

- Marco-polo puzzle (reach and hear for the response)

#### 5.2.4 Enemies

- Smarter enemies that challenge the player to use sound tools and environmental sounds effectively

#### 5.2.5 Puzzles

The Marcopolo puzzle: the market will be a sort of maze through which the player will have to move carefully, using **sound tools** and paying attention to **environmental sounds** as they work as a response system someone is using to guide the player through the area. By following the environmental sounds the player reaches the bell-maker, who provides him with a new sound tool: the **bell**. He also has some information Doctor Kael left behind...

### 5.3. The Resonance Tower

#### 5.3.1 Level Overview

In this level, the player must solve a **complex puzzle** and **apply what was learned** from the information obtained in the previous level.  
This information will allow them to **save the world** by **annihilating the enemies**; this is possible only from the **Resonance Tower**, by properly reprogramming the emitted frequencies.  
**Level ends:** upon completing the reprogramming and neutralizing the threat.

#### 5.3.2 Sound Tools

#### 5.3.3 Environmental Sound Sources

#### 5.3.4 Enemies

#### 5.3.5 Puzzles

## 6. UI / HUD Design

- Main Menu
- Pause Menu
- HUD  
  - **Danger Threshold**: increases with each sound emitted, in accordance with the rules described above.
- Settings Menu
- Inventory

## 7. Assets

To enable the correct functioning of the echolocation mechanism, we are oriented toward using low-poly assets for which there are packages that do not distort our aesthetic vision for the game. In addition, low-poly assets:

- Improve game performance
- Allow greater freedom during environment construction, as they are inherently more modular and reusable
- Facilitate the addition of custom details

Note that we can use to our advantage the fact that most scenes will be completely dark: this can potentially reduce the effort behind the selection of detailed textures and materials which, if necessary, can be omitted.

Available Asset Packages:

- [Simple Apocalypse Interiors - Cartoon Assets](https://assetstore.unity.com/packages/3d/props/interior/simple-apocalypse-interiors-cartoon-assets-38419)
- [Simple Apocalypse - Cartoon Assets](https://assetstore.unity.com/packages/3d/environments/simple-apocalypse-cartoon-assets-44678)

## 8. Soundtrack

We have been contacted by an external composer who offered to create original tracks for the game. Since this is not a priority for the project, if the composer’s participation is not confirmed, royalty-free tracks will be used, sourced from appropriate websites and as consistent as possible with the game’s atmosphere.
In any case, the soundtrack should not be “intrusive,” but rather in the background, so as not to distract the player from environmental sounds and the sound effects related to echolocation.

Soundtrack references:
- [Death Note OST II - "Throb"
](https://youtu.be/qzr_TThucVQ?si=MgH7n9NvJzGpryzK) - from 0:00 to 0:10
- [Death Note OST II - "Suspicious"
](https://youtu.be/OEqej4-ghxU?si=a6M8fP2Y8ilpIGa7) - the drone sound throughout the track


