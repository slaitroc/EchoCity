xw

# ✅ **CURSOR PROMPT — Implement the First Puzzle in EchoCity**

Implement the following puzzle inside the first level ("Noah’s Lab") using the existing architecture of EchoCity (Interactables, Pickables, ObjectFrequencySetter, AudioEmitter, enemy attraction system, etc.).
Use the components and patterns already present in the project instead of creating redundant new systems.

---

## **📌 Puzzle Narrative (for context only, do NOT hardcode text)**

Before entering the actual level, narration explains Noah’s background, why he is inside the bunker, and how he used Kael’s notes to build the echolocation glasses.
Suddenly, the power cuts out, monsters enter the bunker, and Kael’s failsafe seals the entrance door.
Noah must escape and needs:

* Kael’s research data stored on a floppy disk
* A walkie-talkie OR a satellite phone
* A metal bar to force open the final door

All of these items are already present in the scene as props.

---

## **🎯 Required Puzzle Logic**

### **Main linear steps (must be completed in order)**

1. Reactivate the **correct power socket** (the only one missing a cable).
2. Find the **loose cable** in the medic room (Pickable).
3. Plug the cable into the correct socket (Interactable).
4. Power is restored → some devices turn on (fan, lights, ambient hum).
5. Player must disable noisy powered devices to avoid attracting monsters.
6. Use the restored power to operate **one specific floppy disk reader**.
7. Only that reader ejects Kael’s floppy disk when interacted with.
8. Pick up the floppy disk.

### **Parallel non-linear steps (any order)**

* Find either the **walkie-talkie OR the satellite phone** (Pickable).
* Find the **metal bar** in the hidden room (Pickable + new SoundTool).

Puzzle ends when:
→ Player has floppy disk + one communication device + metal bar
→ Player interacts with the bunker exit door using the metal bar
→ Door is forced open → proceed to next scene.

---

## **🛠️ Implementation Requirements**

### **1. Create a central Puzzle Controller**

Implement a `BunkerPuzzleController` or integrate the logic into an existing LevelStateManager.
It must track:

```
hasCable
cablePlugged
floppyEjected
hasFloppy
hasCommsDevice
hasMetalTool
```

And provide:

```
OnCablePickedUp()
OnCablePluggedIn()
OnFloppyEjected()
OnFloppyPickedUp()
OnCommsDevicePickedUp()
OnMetalToolPickedUp()
CanForceExitDoor()
```

Use events/invocations consistent with the existing architecture.

---

### **2. Cable Pickup**

* Convert the cable in the medic room into a Pickable.
* On pickup → call `OnCablePickedUp()`.

Use an `ObjectFrequencySetter` set to **Mid frequency** so it becomes visible only with mid-frequency echolocation.

---

### **3. Sockets**

There are multiple sockets; all appear unplugged, but **only one is actually missing the cable**.
Implement:

#### **Correct socket**

* Interactable
* Only works if player has cable
* When interacted:

  * show the plugged-in cable mesh
  * call `OnCablePluggedIn()`
  * activate devices listed in controller (fan, lights, hum emitters)

#### **Other sockets**

* Interactable
* Always give incorrect feedback (e.g., “This one doesn’t work.”)
* Must not be pluggable.

---

### **4. Powered Devices**

When the socket is activated:

* Power on fan(s), hum emitters, lights, etc.
* These devices must use existing AudioEmitters and frequency logic.
* They should generate enemy attraction, matching normal gameplay rules.
* Player can interact with them to **turn them off**.

These devices are referenced inside the Puzzle Controller and are enabled only after power restoration.

---

### **5. Floppy Disk Readers**

Place multiple floppy disk reader objects.
Only one is valid.

#### **Correct reader**

* Must check `cablePlugged == true`.
* On interact:

  * play the correct animation
  * eject the floppy disk (spawn pickable object)
  * call `OnFloppyEjected()`

#### **All other readers**

* Always respond with “No power” or “Device not working”.

---

### **6. Floppy Disk (Pickable)**

On pickup:

* call `OnFloppyPickedUp()`
* this object should also have an `ObjectFrequencySetter` (High recommended)

---

### **7. Walkie-Talkie / Satellite Phone**

* Both are pickable.
* Picking up **either one** calls `OnCommsDevicePickedUp()`.

---

### **8. Metal Bar (Pickable + SoundTool)**

Found in the hidden room.

On pickup:

* call `OnMetalToolPickedUp()`
* unlock its associated SoundTool (loud metallic strike → High frequency)

---

### **9. Exit Door**

Make the exit door an Interactable.

When interacted:

* Check `controller.CanForceExitDoor()`.
* If false → show feedback.
* If true → play the “force open” animation using the metal bar.
* After animation → trigger the next scene.

---

### **10. Frequency Integration**

Ensure frequency visibility matches EchoCity rules:

* Cable → Mid
* Sockets → Low/Mid
* Floppy reader UI elements → Mid
* Floppy disk → High
* Metal bar detail → Low for geometry, High for emitted sound
* Comms devices → Mid/High

Use existing `ObjectFrequencySetter` components.

---

## **🎬 Final Step**

After the exit door is forced open, proceed to scene transition using the existing `SceneLoader` system.

---

## **✔️ GOAL**

Cursor must integrate this entire puzzle into the existing EchoCity structure **without creating redundant systems**, reusing:

* current Interactable architecture
* current Pickup system
* ObjectFrequencySetter
* AudioEmitter + sound-based enemy attraction
* existing code style and folder organization

