# Reinforcement Learning-Based Robot Navigation in Unity ML-Agents

<p align="center">
  <img src="Results/demo.gif" width="700"/>
</p>

A reinforcement learning research project developed for **TALENTUD-IK** using **Unity ML-Agents**, focused on autonomous robot navigation with PPO, imitation learning, and LiDAR-style sensing.

---

# Project Demo

### Robot Navigation Training Demo

<p align="center">
  <img src="Results/demo.gif" width="700"/>
</p>

The humanoid RobotAgent learns to:

- Navigate toward a target
- Avoid obstacles and walls
- Use LiDAR-style ray perception
- Improve behavior through reinforcement learning and imitation learning

---

# Project Overview

This project investigates how reinforcement learning can be used for autonomous robot navigation inside a Unity simulation environment.

A humanoid robot agent learns to:

- Reach a target object
- Avoid collisions
- Use sensor-based perception
- Learn from demonstrations

The project was developed using **Unity 2022.3 LTS** and **Unity ML-Agents**.

---

# Research Goals

The project focuses on:

- Reinforcement learning for robot navigation
- PPO (Proximal Policy Optimization)
- Behavioral Cloning (BC)
- Generative Adversarial Imitation Learning (GAIL)
- LiDAR simulation using RayPerceptionSensor3D
- Reward engineering and reward exploit correction
- Sim-to-real transfer learning discussion

---

# Technologies Used

| Technology | Description |
|---|---|
| Unity 2022.3 LTS | Simulation environment |
| Unity ML-Agents | Reinforcement learning toolkit |
| Python 3.10 | Training environment |
| PyTorch | Neural network backend |
| PPO | Main reinforcement learning algorithm |
| Behavioral Cloning | Imitation learning |
| GAIL | Adversarial imitation learning |
| RayPerceptionSensor3D | Simulated LiDAR sensing |
| TensorBoard | Training visualization |

---

# Environment Design

The simulation environment contains:

- Floor
- Four boundary walls
- Obstacles
- Target object
- Humanoid RobotAgent

The target position is randomized each episode to improve generalization.

---

# RobotAgent Setup

The robot uses:

- Continuous movement actions
- Rigidbody physics
- Animator Controller
- RayPerceptionSensor3D

## Actions

| Action | Purpose |
|---|---|
| Move | Forward / backward |
| Rotate | Left / right turning |

---

# Observation Space

The agent receives:

- Direction to target
- Distance to target
- Local velocity
- Forward direction
- Movement speed
- RayPerceptionSensor3D observations

Total vector observations: **12**

---

# LiDAR Sensor Integration

RayPerceptionSensor3D was used to simulate LiDAR-like sensing.

The sensor detects:

- Walls
- Obstacles
- Target

Configuration:

- 8 rays per direction
- 90-degree detection angle

This improves obstacle awareness and navigation stability.

---

# Reward Function

Final reward structure:

| Event | Reward |
|---|---:|
| Time penalty | -0.001 |
| Looking toward target | Small positive reward |
| Reaching target | +20 |
| Collision | -0.5 |

---

# Reward Exploit Fix

An earlier reward design included a continuous distance reward.

This caused the agent to exploit the reward system by staying near the target without completing the task.

The issue was solved by:

- Removing dense distance reward
- Increasing terminal target reward

This produced more meaningful navigation behavior.

---

# PPO Training Results

## PPO Without RayPerceptionSensor3D

- Lower cumulative reward
- Less stable navigation
- More collisions

## PPO With RayPerceptionSensor3D

- Higher cumulative reward
- Improved obstacle avoidance
- More stable navigation behavior

<p align="center">
  <img src="Results/ppo_training_comparison_with_rayperception_sensor.png" width="900"/>
</p>

---

# Sim-to-Real Discussion

The project also discusses:

- Simulation-to-real transfer
- Sensor noise
- Physics mismatch
- Domain randomization
- Transfer learning

Potential real-world equivalents of RayPerceptionSensor3D include:

- LiDAR
- Ultrasonic sensors
- Depth sensors

---

# Project Structure

```text
Assets/
├── Scripts/
│   └── RobotAgent.cs
│
├── Demonstrations/
│   └── RobotNavDemo_v2.demo
│
├── Scenes/
│   └── TrainingArea.unity
│
├── Models/
│   └── Trained models
│
├── Sensors/
│   └── RayPerception setup
│
Packages/
ProjectSettings/

robot_nav.yaml
README.md
```

---

# How to Run

## Unity

1. Open project in Unity 2022.3 LTS
2. Open the training scene
3. Ensure ML-Agents package is installed

## Python Environment

```bash
conda create -n mlagents python=3.10
conda activate mlagents
pip install mlagents torch
```

## Train PPO

```bash
mlagents-learn robot_nav.yaml --run-id=RobotNav_PPO
```

Then press **Play** in Unity.

---

# Presentation

The repository also contains:

- Final TALENTUD presentation (PPTX)
- PDF presentation
- Training graphs
- Demo GIF/video

---

# Limitations

- Low-resource hardware
- Simplified humanoid movement
- No real robot deployment
- SAC comparison limited by hardware
- DQN not implemented due to continuous actions

---

# Future Work

- SAC comparison on stronger hardware
- Domain randomization
- Dynamic obstacles
- Camera/depth sensors
- Real robot deployment
- Sensor noise modeling

---

# Author

**Azish Ahmed**  
TALENTUD-IK Research Project  
University of Debrecen

---

# References

1. Unity ML-Agents Toolkit Documentation
2. Proximal Policy Optimization Algorithms
3. Generative Adversarial Imitation Learning
4. Reinforcement Learning: An Introduction
5. Soft Actor-Critic
