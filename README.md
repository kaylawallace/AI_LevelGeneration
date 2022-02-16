# AI_LevelGeneration
--------------------------------------------------------------------------------
Unity Version: 2021.2.5f1

--------------------------------------------------------------------------------

Instructions:
- Add the LevelGenerationAI project to your Unity Hub and open
- Locate the 'Models' folder in the 'Assets' folder 
    -This folder contains 4 .onnx models that you can use to see the 
     project in action. 
     
	   - PPO_NoRewardShaping - agent trained using PPO and no reward shaping
	   - PPO_RewardShaping - agent trained using PPO with reward shaping 
	   - SAC_NoRewardShaping - agent trained using SAC with no reward shaping 
	   - SAC_RewardShaping - agent trained using SAC with reward shaping 


- Locate the 'LevelGenAgent' in each of the three 'TrainingArea's
- Locate the 'BehaviourParameters' component of the 'LevelGenAgent' and drag in 
  a model of your choosing from the 4 .onnx models. 
- Press 'Play' in the Unity Editor and watch the agents traverse through their 
  respective tile grid, making decisions as they go.

--------------------------------------------------------------------------------
