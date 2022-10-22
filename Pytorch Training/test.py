import argparse

import torch
import torch.optim as optim
import torch.nn.functional as F
from torch.utils.data import DataLoader

from model import HandModel
from data import HandDataset

GPU_indx = 0
device = torch.device(GPU_indx if torch.cuda.is_available() else 'cpu')

def test(model: HandModel, dataset: HandDataset):
    model.eval()
    with torch.no_grad():
        for i in range(len(dataset)):

            filename = dataset.filenames[i]

            image, action, position, prev_action, prev_position = dataset[i]

            image = image[None, :]
            action = torch.unsqueeze(action, 0)
            position = position[None, :]
            prev_action = torch.unsqueeze(prev_action, 0)
            prev_position = prev_position[None, :]

            pred_action, pred_position = model(image.to(device), prev_action.to(device), prev_position.to(device))
            action, position = action.to(device), position.to(device)

            predicted_action = torch.argmax(pred_action, dim=1)

            print(f"({i}/{len(dataset)}) {filename}: \t Predicted Action: {predicted_action.item()}% \t Real Action {action.item()} \t Position Difference: {pred_position - position}")

model = HandModel().to(device)

check_point = torch.load("test9.pt")
model.load_state_dict(check_point['model_state_dict'])

test_dataset = HandDataset(dirs = ["../Data/Validation"], training = False)
test(model, test_dataset)