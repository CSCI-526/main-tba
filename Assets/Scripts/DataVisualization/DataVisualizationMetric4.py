import json
import requests
import matplotlib.pyplot as plt
import numpy as np
from collections import Counter

url = 'https://botorbought-85297-default-rtdb.firebaseio.com/workbench_sales.json' 

response = requests.get(url)
data = response.json()

# Filter data where all soldCardNames have the same color
robot_sell_weapon_use = []
for entry in data.values():
    if "soldCardNames" in entry and isinstance(entry["soldCardNames"], list) and entry["soldCardNames"]:  # Check if key exists & is a non-empty list
        #card_colors = {card[-2:] for card in entry["soldCardNames"]}
        card_parts = []
        for card in entry["soldCardNames"]:
            if card[:4] == "Head":
                card_parts.append("H")
            elif card[:4] == "Left":
                if card[4:5] == "F":
                    card_parts.append("LF")
                else:
                    card_parts.append("LA")
            elif card[:4] == "Righ":
                if card[5:6] == "F":
                    card_parts.append("RF")
                else:
                    card_parts.append("RA")
        #print(len(set(card_parts)))  

        #if len(card_colors) == 1:  # All cards must have the same color
        if len(set(card_parts)) == 1 and len(card_parts) != 1:
            robot_sell_weapon_use.append("W")
        elif len(set(card_parts)) != 1 and len(card_parts) != 1 and entry["totalPoints"] != 0:
            robot_sell_weapon_use.append("R")



# Count occurrences of each totalPoints value
robots_weapons_count = Counter(robot_sell_weapon_use)
#print(robots_weapons_count)

# Prepare data for plotting
robot_or_weapon, counts = zip(*sorted(robots_weapons_count.items()))  # Sorted by points value


# Create the plot
fig, ax = plt.subplots()
index = np.arange(1)  # Single bar at position 0
bar_width = 0.5

# Plot stacked bars (1 bar: blue bottom, orange on top)
robot_count = counts[0]
weapon_count = counts[1]


#ax.bar(index, counts, bar_width, color='blue')
bars1 = ax.bar(index, robot_count, width=bar_width, color='blue', label='Robot')
bars2 = ax.bar(index, weapon_count, width=bar_width, bottom=robot_count, color='orange', label='Weapon')

ax.set_xlim(-0.8, 0.8)

# Add total label on top
#total = robot_count + weapon_count
#ax.text(index[0], total + 0.1, str(total), ha='center', va='bottom')

# Final touches
ax.set_ylabel('Count of Robot Selling and Weapons Using')
ax.set_title('Robot Selling vs Weapons Using')
ax.set_xticks([])
ax.legend()

# Save and show the plot
plt.savefig("metric4.png")
plt.show()
