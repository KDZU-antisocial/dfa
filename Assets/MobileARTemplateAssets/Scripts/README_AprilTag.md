# AprilTag Integration for AR Foundation

This document explains how to use the AprilTag tracking system that has been integrated into your AR Foundation project.

## Overview

The AprilTag integration provides robust marker-based tracking for AR applications. AprilTags are 2D barcode-like markers that can be detected and tracked by the camera, providing precise pose estimation for AR content placement.

## Components

### 1. AprilTagManager
The main component that manages AprilTag detection and tracking.

**Key Features:**
- Detects AprilTags in real-time
- Manages AprilTag visualizations
- Provides events for tag detection/loss
- Configurable detection threshold and limits

**Setup:**
1. Add the `AprilTagManager` component to a GameObject in your scene
2. Assign the AR Camera to the `Ar Camera` field
3. Assign the AprilTag prefab to the `April Tag Prefab` field
4. Configure detection settings as needed

### 2. AprilTagVisualization
Handles the visual representation of detected AprilTags.

**Key Features:**
- Updates position and rotation based on tag detection
- Shows tag ID as text overlay
- Configurable scale and appearance
- Automatic camera-facing text

**Setup:**
- Automatically added to AprilTag prefabs
- Configure scale and text display in the inspector

### 3. AprilTagSetup
Helper component for easy setup of the AprilTag system.

**Key Features:**
- Automatic detection of AR Camera and Session Origin
- Creates AprilTag detector component
- Configures the AprilTag manager
- Provides test functionality

**Usage:**
1. Add `AprilTagSetup` to any GameObject
2. Click "Setup AprilTag Detection" in the inspector
3. Use "Test AprilTag Detection" to enable detection

### 4. AprilTagGenerator
Utility for generating AprilTag images for testing.

**Key Features:**
- Generate single or multiple AprilTag images
- Configurable tag family and size
- Save images to project folder
- Create test scenes

**Usage:**
1. Add `AprilTagGenerator` to any GameObject
2. Configure generation settings
3. Use context menu options to generate tags

## Quick Start

### 1. Basic Setup
1. Open your AR scene
2. Add an empty GameObject and attach the `AprilTagSetup` component
3. In the inspector, click "Setup AprilTag Detection"
4. The system will automatically configure itself

### 2. Generate Test Tags
1. Add the `AprilTagGenerator` component to any GameObject
2. Right-click the component and select "Generate Multiple AprilTags"
3. Print the generated images or display them on a screen
4. Point your AR camera at the tags to test detection

### 3. Customize Visualizations
1. Create a custom prefab for AprilTag visualization
2. Add the `AprilTagVisualization` component
3. Assign your custom prefab to the AprilTag Manager
4. Configure the visualization settings

## Configuration

### Detection Settings
- **Detection Threshold**: Minimum confidence for tag detection (0-1)
- **Max Tracked Tags**: Maximum number of tags to track simultaneously
- **Show Debug Info**: Enable console logging for debugging

### Visualization Settings
- **Scale**: Size multiplier for tag visualizations
- **Show Tag ID**: Display the tag ID as text
- **Tag ID Text**: UI Text component for displaying the ID

## Events

The AprilTag system provides several events for integration:

```csharp
// Subscribe to events
AprilTagManager manager = FindObjectOfType<AprilTagManager>();
manager.OnAprilTagDetected += (tagId, detection) => {
    Debug.Log($"Tag {tagId} detected!");
};
manager.OnAprilTagLost += (tagId) => {
    Debug.Log($"Tag {tagId} lost!");
};
manager.OnAprilTagUpdated += (tagId, detection) => {
    Debug.Log($"Tag {tagId} updated!");
};
```

## API Reference

### AprilTagManager
- `GetTrackedTagIds()`: Get array of currently tracked tag IDs
- `GetAprilTagVisualization(int tagId)`: Get visualization for specific tag
- `ClearAllAprilTags()`: Remove all tracked tags
- `SetDetectionEnabled(bool enabled)`: Enable/disable detection

### AprilTagVisualization
- `GetWorldPosition()`: Get world position of the tag
- `GetWorldRotation()`: Get world rotation of the tag
- `GetDistanceFromCamera()`: Get distance from AR camera
- `SetVisible(bool visible)`: Show/hide the visualization

## Troubleshooting

### Common Issues

1. **No tags detected**
   - Check that the AprilTagDetector is attached to the AR Camera
   - Verify the detection threshold is not too high
   - Ensure good lighting and tag visibility

2. **Tags not positioned correctly**
   - Check that the AR Camera is assigned correctly
   - Verify the tag family matches the generated tags
   - Ensure the tag is flat and not wrinkled

3. **Performance issues**
   - Reduce the maximum number of tracked tags
   - Lower the detection threshold
   - Check device performance capabilities

### Debug Tips

1. Enable "Show Debug Info" in the AprilTagManager
2. Use the AprilTagGenerator to create test tags
3. Check the console for detection messages
4. Verify tag images are clear and properly printed

## Advanced Usage

### Custom Tag Families
The system supports different AprilTag families. Change the family in the AprilTagGenerator to generate different types of tags.

### Custom Visualizations
Create custom prefabs with your own 3D models, animations, or UI elements. The `AprilTagVisualization` component will handle positioning automatically.

### Integration with Existing Systems
The AprilTag system can be integrated with your existing AR systems:
- Use detected tags to anchor AR content
- Trigger actions when specific tags are detected
- Combine with plane detection for hybrid tracking

## Performance Considerations

- AprilTag detection is computationally intensive
- Consider limiting the number of tracked tags on mobile devices
- Use appropriate detection thresholds for your use case
- Test on target devices for performance validation

## Support

For issues or questions about the AprilTag integration:
1. Check the Unity console for error messages
2. Verify all components are properly configured
3. Test with generated AprilTag images first
4. Check the AprilTag package documentation for advanced features
