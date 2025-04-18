import json
import requests
import matplotlib.pyplot as plt
import numpy as np
from collections import Counter

url = 'https://botorbought-85297-default-rtdb.firebaseio.com/workbench_sales.json' 

response = requests.get(url)
data = response.json()

# Filter data where all soldCardNames have the same color
filtered_weapons = []
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
        #print(card_parts)	

        #if len(card_colors) == 1:  # All cards must have the same color
        if len(set(card_parts)) == 1 and len(card_parts) != 1:
            filtered_weapons.append(card_parts[0])
            #print(card_parts)


# Count occurrences of each totalPoints value
weapons_count = Counter(filtered_weapons)

# Prepare data for plotting
weapons, counts = zip(*sorted(weapons_count.items()))  # Sorted by points value

# Create the plot
fig, ax = plt.subplots()
index = np.arange(len(weapons))
bar_width = 0.5

ax.bar(index, counts, bar_width, color='blue')
ax.set_xlabel('Weapons')
ax.set_ylabel('Count of Weapons Using')
ax.set_title('Distribution of Weapons Using')
ax.set_xticks(index)
ax.set_xticklabels(weapons)

# Save and show the plot
plt.savefig("metric2.png")
plt.show()
