import json
import requests
import matplotlib.pyplot as plt
import numpy as np
from collections import Counter

url = 'https://botorbought-85297-default-rtdb.firebaseio.com/game_turns.json' 

response = requests.get(url)
data = response.json()

total_turns_list = [entry["totalTurns"] for entry in data.values()]

x = np.arange(1, len(total_turns_list)+1)  # X-axis values (e.g., time, index)
print(len(total_turns_list))
total_turns = np.array(total_turns_list)
print(total_turns)

# Calculate the average
average_value = np.mean(total_turns)

# Create the plot
plt.figure(figsize=(8, 5))
plt.plot(x, total_turns, marker='o', linestyle='-', color='blue', label='Data Line')  # Line chart
plt.axhline(average_value, color='red', linestyle='--', label=f'Average ({average_value:.2f})')  # Average line
plt.xticks([])

# Labels and title
plt.ylabel('Total turns')
plt.title('Total turns of a game')
plt.legend()
plt.grid(True)

# Save and show the plot
plt.savefig("metric3.png")
plt.show()
