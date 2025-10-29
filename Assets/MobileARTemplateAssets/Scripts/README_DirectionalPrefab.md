# Directional AprilTag Prefab Guide

## What Each Color Means:

### 🟩 **GREEN ARROW** = UP
- Points upward from the top of the tag
- Shows which way is "up" on the AprilTag

### 🔵 **BLUE DOT** = FRONT
- Small cylinder on the front face
- Shows which side of the tag is facing the camera

### 🔴 **RED ARROW** = ROTATION/RIGHT
- Points to the right side
- Shows the rotation direction (0°, 90°, 180°, 270°)

### 🟡 **YELLOW SPHERE** = BOTTOM-LEFT CORNER
- Marks the bottom-left corner
- Reference point for orientation

## How to Create the Prefab:

### Method 1: Using Menu (After Unity Compiles)
1. Wait for Unity to compile the new script
2. Go to **GameObject → AprilTag → Create Directional AprilTag Prefab**
3. Done! The prefab is automatically created and ready to use

### Method 2: Manual Creation (If Menu Doesn't Work)
1. In Unity, right-click in **Hierarchy**
2. Create **Create Empty** and name it "AprilTagPrefab"
3. Add children to it:
   - **Cube** (scaled to 0.1, 0.1, 0.005) - white base
   - **Cube** (scaled to 0.015, 0.01, 0.003, position 0, 0.035, 0.005) - green arrow (UP)
   - **Cylinder** (scaled to 0.02, 0.005, 0.02, position 0, 0, 0.01) - blue dot (FRONT)
   - **Cube** (scaled to 0.015, 0.01, 0.003, position 0.04, 0, 0.005) - red arrow (RIGHT)
   - **Sphere** (scaled to 0.015, position -0.045, -0.045, 0.008) - yellow corner
4. Add **AprilTagVisualization** component to the parent
5. Drag to Prefabs folder

## How to Use:

1. Once created, assign this prefab to your **AprilTag Manager**
2. When AprilTags are detected, you'll see clear visual indicators:
   - Green arrow points up
   - Blue dot shows the front face
   - Red arrow shows right/rotation
   - Yellow sphere marks the corner

## Testing Orientation:

- **Rotate the physical AprilTag** → Red arrow changes direction
- **Flip the tag upside down** → Green arrow points down
- **Face tag away from camera** → Blue dot disappears (back side)

This makes it super easy to debug pose estimation and orientation!
