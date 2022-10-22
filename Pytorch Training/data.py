import json
import os
import random
import torch
from PIL import Image
from torch.utils.data import DataLoader, Dataset, random_split
import torchvision.transforms as transforms

class HandDataset(Dataset):
    def __init__(self, dirs, training: bool, filter = [0, 1, 2, 3], limit: int = None):

        self.filenames = []
        self.actions = []
        self.positions = []
        
        if training:
            self.transform = transforms.Compose([
                transforms.ToTensor(),
                transforms.Resize((120, 160)),
                transforms.ColorJitter(.3, .3, .3, .05),
            ])
        else:
            self.transform = transforms.Compose([
                transforms.ToTensor(),
                transforms.Resize((120, 160))
            ])

        for dir in dirs:
            for root, _, files in os.walk(dir):
                root = root.replace("\\","/")
                for file in files:
                    if file == 'data.json':
                        self.load_data(root)

        self.actions = torch.tensor(self.actions).type(torch.LongTensor)
        self.positions = torch.tensor(self.positions)

        if limit != None and limit < len(self.filenames):
            self.filenames = self.filenames[0:limit]
            self.actions = self.actions[0:limit]
            self.positions = self.positions[0:limit]
    
    def load_data(self, directory):
        with open(directory + '/data.json') as file:
            data = json.load(file)
            filenames = list(map(lambda image: directory + "/" + image['filename'], data))
            actions = list(map(lambda image: image['action'], data))
            positions = list(map(lambda image: [float(image['x']), float(image['y']), float(image['z'])], data))
            self.filenames.extend(filenames)
            self.actions.extend(actions)
            self.positions.extend(positions)
    
    def __len__(self):
        return len(self.filenames)
    
    def __getitem__(self, index):
        image = Image.open(self.filenames[index])
        image = self.transform(image)

        action = self.actions[index]
        position = self.positions[index]

        prev_action = action.type(torch.float) if random.random() > 0.25 else torch.randint(0, 4, (1,))[0].type(torch.float)
        prev_position = position + torch.normal(torch.tensor([0.0, 0.0, 0.0]), torch.tensor([0.25, 0.25, 0.4]))

        #prev_position = self.positions[max(index - 1, 0)] * 0
        #prev_action = self.actions[max(index - 1, 0)].type(torch.float) * 0

        return image, action, position, prev_action, prev_position
