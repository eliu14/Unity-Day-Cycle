# Unity-Day-Cycle

A project that allows the user to control the day time via a clock.


## To import into your project

1. Create an empty object called TimeController
2. Add the DayCycle script to it
3. Assign the following 
	- Time Multiplier
		- This is a speed factor for how fast the day will go by. This variable is a float.
	- Start Hour
		- This is a float variable that represents the hour of the day in 24 hour format that the time will start at.
	- Time Text
		- This is the text object that will display the current time in game.
	- Sun Light
		- This is the directional light that will be shined during the day.
	- Moon Light
		- This is the directional light that will be shined during the night.
	- Sunrise Hour
		- This is the hour, in 24 hour format, that marks sunrise
	- Sunset Hour
		- This is the hour, in 24 hour format, that marks sunset
	- Day Ambient Light
		- This is the ambient color that will be visible during the day time.
	- Night Ambient Light
		- This is the ambient color that will be visible during the night time.
	- Light Change Curve
		- This is the curve that controls the transition between the sunlight and moonlight.
	- Maximum Sunlight Intensity
		- The maximum intensity of the sunlight.
	- Maximum Moonlight Intensity
		- the maximum intensity of the moonlight.
4. 