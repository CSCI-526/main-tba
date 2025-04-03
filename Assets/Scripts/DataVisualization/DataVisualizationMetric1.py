import json
import requests
import matplotlib.pyplot as plt
import numpy as np
from collections import Counter

url = 'https://botorbought-85297-default-rtdb.firebaseio.com/workbench_sales.json' 

response = requests.get(url)
data = response.json()

def all_elements_same(lst):
    return len(set(lst)) == 1

# Filter data where all soldCardNames have the same color
filtered_total_points = []
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
        		if card[4:5] == "F":
        			card_parts.append("RF")
        		else:
        			card_parts.append("RA")
        #print(card_parts)	

        #if len(card_colors) == 1:  # All cards must have the same color
        if len(set(card_parts)) != 1 and entry["totalPoints"] != 0:
            filtered_total_points.append(entry["totalPoints"])


# Extract totalPoints values
#total_points_list = [entry["totalPoints"] for entry in data.values()]

# Count occurrences of each totalPoints value
points_count = Counter(filtered_total_points)

# Prepare data for plotting
points, counts = zip(*sorted(points_count.items()))  # Sorted by points value
#print(points_count)

# Create the plot
fig, ax = plt.subplots()
index = np.arange(len(points))
bar_width = 0.5

ax.bar(index, counts, bar_width, color='blue')
ax.set_xlabel('Points')
ax.set_ylabel('Count of points')
ax.set_title('Distribution of Points')
ax.set_xticks(index)
ax.set_xticklabels(points)

# Save and show the plot
plt.savefig("metric1.png")
plt.show()
