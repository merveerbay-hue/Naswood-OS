# Animation

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Animation standard defines how motion is used throughout Naswood OS.

Animations improve usability by providing visual feedback, guiding user attention and making interface interactions feel natural without distracting users.

Animations are intended to support productivity, not decoration.

---

# Objectives

- Improve User Experience
- Provide Visual Feedback
- Guide User Attention
- Communicate State Changes
- Increase Perceived Performance
- Maintain Consistency
- Preserve Accessibility

---

# Design Principles

Animations must be:

- Fast
- Meaningful
- Consistent
- Predictable
- Smooth
- Optional for accessibility

---

# Animation Philosophy

Animation should never slow down business operations.

The interface must always prioritize speed over visual effects.

Every animation must have a purpose.

---

# Animation Categories

Micro Interactions

Page Transitions

Loading Animations

Notifications

Dialogs

Navigation

Expansion

Selection

Hover Effects

Drag & Drop

---

# Duration Standards

Very Fast

100 ms

---

Fast

150 ms

---

Normal

200 ms

---

Medium

250 ms

---

Slow

300 ms

Maximum recommended duration.

---

# Easing

Default

ease-out

---

Entry

ease-out

---

Exit

ease-in

---

Interactive

ease-in-out

---

Spring animations may be used only for drag operations.

---

# Standard Animations

Fade In

Fade Out

Slide Up

Slide Down

Slide Left

Slide Right

Scale In

Scale Out

Expand

Collapse

Rotate

Pulse

Shake (Validation only)

---

# Page Transitions

Page transitions should be subtle.

Recommended duration

200 ms

Avoid dramatic movement.

---

# Modal Animation

Open

Fade + Scale

Close

Fade

Maximum duration

200 ms

---

# Sidebar Animation

Expand

200 ms

Collapse

200 ms

Do not animate menu content individually.

---

# Navigation

Active menu item

Background transition

Icon transition

Text fade

Maximum duration

150 ms

---

# Button Animation

Hover

Background transition

Pressed

Scale 98%

Focus

Outline animation

Disabled

No animation

---

# Input Animation

Focus border

150 ms

Validation

Fade

Error

Shake (optional)

Success

Fade

---

# Table Animation

Row Hover

Background fade

Sorting

Smooth transition

Loading

Skeleton

Pagination

Fade

---

# Card Animation

Hover Elevation

Lift slightly

Click

Press animation

Loading

Skeleton

---

# Notification Animation

Appear

Slide Down

Disappear

Fade Out

Duration

250 ms

---

# Loading States

Skeleton Loading

Preferred

Spinner

Secondary

Progress Bar

Long operations

---

# Skeleton Guidelines

Use skeletons instead of spinners whenever possible.

Skeletons should match final layout.

---

# Drag and Drop

Smooth movement

Drop indicator

Snap animation

Maximum duration

200 ms

---

# Charts

Chart appearance

Fade

Tooltip

Fade

Legend

No animation

Real-time updates

Smooth interpolation

---

# AI Components

Typing Indicator

Fade

Streaming Response

Progressive appearance

Suggestion Cards

Fade Up

---

# Mobile

Reduce motion.

Avoid large movements.

Support touch gestures.

Maintain 60 FPS.

---

# Accessibility

Respect operating system settings.

Support

prefers-reduced-motion

If enabled

Disable

Parallax

Zoom

Bounce

Complex transitions

---

# Performance

Target

60 FPS

Avoid

Heavy blur

Large shadows

Complex SVG animations

Layout recalculations

---

# CSS Recommendations

Prefer

transform

opacity

Avoid animating

width

height

top

left

margin

padding

---

# GPU Optimized Properties

transform

opacity

filter (limited)

---

# Animation Tokens

animation-fast

animation-normal

animation-slow

animation-fade

animation-slide

animation-scale

animation-bounce

---

# Interaction Feedback

Every user interaction should provide feedback.

Examples

Button Press

Hover

Selection

Drag

Drop

Expand

Collapse

Save

Delete

Upload

Download

---

# Motion Guidelines

Motion should direct attention.

Do not animate decorative elements repeatedly.

Avoid infinite animations except loading indicators.

---

# Error Animation

Use minimal shake animation.

Maximum one repetition.

Never combine shake with flashing.

---

# Success Animation

Fade

Checkmark

Color transition

Duration

200 ms

---

# Responsive Behaviour

Desktop

Full animation

Tablet

Reduced animation

Mobile

Minimal animation

Low Power Devices

Disable complex effects

---

# Browser Support

Chrome

Edge

Firefox

Safari

---

# Best Practices

Animate only when necessary.

Keep animations under 300 ms.

Prefer opacity and transform.

Maintain consistent timing.

Use subtle transitions.

Test on low-end devices.

Respect accessibility settings.

---

# Do

✓ Use Fade

✓ Use Slide

✓ Use Scale

✓ Keep transitions fast

✓ Test performance

✓ Use Skeleton Loading

---

# Don't

✗ Use long animations

✗ Use flashing effects

✗ Animate every element

✗ Use unnecessary bounce effects

✗ Block user interaction

✗ Ignore reduced motion settings

---

# Acceptance Criteria

Animations are smooth.

No animation exceeds 300 ms.

UI remains responsive.

Accessibility settings are respected.

Animations use GPU-friendly properties.

Performance remains above 60 FPS.

Loading states are visually consistent.

Animations improve usability without distracting users.

---

# Related Documents

Accessibility.md

Design_Tokens.md

Buttons.md

Inputs.md

Dialogs.md

Sidebar.md

Navigation.md

Dashboard.md

Responsive.md
