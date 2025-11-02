# Improving AprilTag Detection at Angles

Your AR image tracking is now working! Here are proven techniques to improve detection at difficult angles:

## 🎯 Quick Wins

### 1. **Image Quality**
- ✅ **High Resolution**: Use at least 1024x1024px for your reference images
- ✅ **High Contrast**: Ensure your printed AprilTag has strong black/white contrast
- ✅ **Sharp Edges**: Avoid blur or pixelation in the printed tag

### 2. **Physical Setup**
```
GOOD Angles:          BAD Angles:
  📱 (30-90°)          📱 (<15°)
  ⬇️                   ⬇️↘️
  🏷️                    🏷️

Direct or moderate    Extreme shallow angle
```

### 3. **Environmental Factors**
- **Lighting**: Bright, even lighting (avoid shadows on the tag)
- **Surface**: Flat, non-reflective surface (matte finish best)
- **Distance**: Stay within 20cm - 2m from the tag
- **Movement**: Slow, steady camera movement (not rapid panning)

## ⚙️ Technical Improvements

### Option 1: Use the Enhanced Script (Included)

Add `ImprovedImageTracking.cs` to your ARTrackedImageManager GameObject:

1. In Unity, select the GameObject with `ARTrackedImageManager`
2. Add Component → `ImprovedImageTracking`
3. Configure:
   - **Max Moving Images**: `2` (or `1` for best performance)
   - **Enable Auto Scale**: `true` ✅
   - **Min Detection Quality**: `0.2-0.3` (lower = more lenient)

This script will:
- Monitor tracking quality in real-time
- Log warnings for poor viewing angles
- Provide quality metrics for debugging

### Option 2: Improve Reference Images

**Current**: `0-AprilTag-4x4-70mm.png` (probably 512x512 or less)

**Better**: Create high-resolution versions
```bash
# If you have the original AprilTag, scan it at high resolution
# OR generate new ones at 2048x2048px
```

**In Unity:**
1. Select your AprilTag image in Project window
2. Inspector → Texture Import Settings:
   - Max Size: `2048`
   - Compression: `None` or `High Quality`
   - Filter Mode: `Bilinear` or `Trilinear`
3. Apply and rebuild the Reference Image Library

### Option 3: Increase Max Number of Moving Images

In your scene (`XR Origin (AR Rig)` GameObject):
- Select `ARTrackedImageManager` component
- Set `Max Number Of Moving Images` to `3-4` (may impact performance)

This allows AR Foundation to track more potential matches simultaneously.

## 📊 Understanding Detection Quality

The `ImprovedImageTracking` script estimates quality based on viewing angle:

| Quality | Angle Range | Detection   | Action Needed            |
|---------|-------------|-------------|--------------------------|
| 1.0-0.7 | 60-90°      | ✅ Excellent | None                     |
| 0.7-0.4 | 30-60°      | ⚠️ Good     | Works, but track quality |
| 0.4-0.2 | 15-30°      | ⚠️ Poor     | Adjust angle if possible |
| <0.2    | <15°        | ❌ Very Poor | Likely to lose tracking  |

## 🚀 Performance Tips

**If tracking is slow or stuttering:**

1. **Reduce Max Moving Images** to `1` (tracks only the closest/best match)
2. **Lower Reference Image Resolution** to `1024x1024`
3. **Disable Auto Scale Estimation** (faster, but worse angle detection)
4. **Ensure good lighting** (poor lighting = slow processing)

## 🔬 Advanced: Multiple Reference Angles (Overkill)

For extreme angles, you can add multiple versions of the same AprilTag at different perspectives:

1. Take photos of your AprilTag from different angles
2. Add all versions to Reference Image Library with same name
3. AR Foundation will match any version

**⚠️ Usually not needed** - the default system handles moderate angles well.

## 🎯 Real-World Testing Tips

**Best practices for reliable detection:**

1. **Start Perpendicular**
   - Hold phone directly above/in front of tag
   - Wait for initial detection (model appears)
   - Then slowly move to test angles

2. **Good Lighting is Critical**
   - Overhead lighting or natural daylight
   - Avoid backlighting (window behind tag)
   - No shadows on the tag surface

3. **Physical Tag Quality**
   - Print on thick, flat cardstock
   - Use high-quality printer (laser preferred)
   - Laminate if possible (prevents wear, adds rigidity)

4. **Camera Movement**
   - Slow, smooth movements
   - Avoid rapid panning or rotation
   - Give the system time to adjust (0.5-1 second)

## 📱 Device-Specific Considerations

**ARKit (iOS):**
- Generally excellent angle detection
- Better in good lighting
- LiDAR devices (iPhone 12 Pro+) have slight advantage

**ARCore (Android):**
- Performance varies by device
- Flagship devices perform best
- May need higher quality reference images

## 🐛 Troubleshooting

**"Image detected briefly then lost at angles"**
→ Lower `m_MinimumDetectionQuality` in `ImprovedImageTracking`

**"No detection at any angle"**
→ Check reference image matches physical tag exactly

**"Jittery/unstable tracking at angles"**
→ Improve lighting, ensure flat surface, slow camera movement

**"Works great straight-on, terrible at angles"**
→ Enable automatic scale estimation, increase max moving images

---

## Summary: Best Configuration for Angle Detection

```csharp
// In ImprovedImageTracking.cs:
m_MaxNumberOfMovingImages = 2;           // Balance performance/detection
m_EnableAutomaticImageScaleEstimation = true;  // ✅ Enable for angles
m_MinimumDetectionQuality = 0.2f;        // Lenient for difficult angles
```

**Physical setup:**
- High-contrast printed tag
- Matte surface, good lighting
- 30-90° viewing angles work best

**You're all set!** 🎉

