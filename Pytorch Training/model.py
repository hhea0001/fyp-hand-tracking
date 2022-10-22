import torch
import torch.nn as nn

class SimpleConv(nn.Module):
    def __init__(self, in_channels, out_channels, pool=True):
        super(SimpleConv, self).__init__()
        
        self.max = nn.MaxPool2d(2, 2)
        self.conv = nn.Conv2d(in_channels=in_channels, out_channels=out_channels, kernel_size=3, padding=1)

        if pool:
            self.down = nn.Sequential(self.max, self.conv)
        else:
            self.down = self.conv

        self.relu = nn.ReLU()
        
    def forward(self, x):
        x = self.down(x)
        x = self.relu(x)
        return x

class HandModel(nn.Module):
    def __init__(self):
        super(HandModel, self).__init__()
        self.down = nn.Sequential(
            SimpleConv(3, 18),
            SimpleConv(18, 36),
            SimpleConv(36, 81),
            SimpleConv(81, 127),
            SimpleConv(127, 64, pool=False),
            SimpleConv(64, 64, pool=False),
            nn.Flatten()
        )
        self.linear = nn.Sequential(
            nn.Linear(4484, 512),
            nn.ReLU(),
            nn.Linear(512, 256),
            nn.ReLU(),
        )
        self.action = nn.Sequential(
            nn.Linear(256, 64),
            nn.ReLU(),
            nn.Linear(64, 4)
        ) 
        self.position = nn.Sequential(
            nn.Linear(256, 64),
            nn.ReLU(),
            nn.Linear(64, 3)
        )
        self.prev_action = nn.Linear(1, 256)
        self.prev_position = nn.Linear(3, 256)

    def forward(self, image, prev_action, prev_position):
        x = self.down(image)
        x = torch.cat((x, prev_action[:,None], prev_position), dim=1)
        x = self.linear(x)# + self.prev_action(prev_action[:,None]) + self.prev_position(prev_position)
        action = self.action(x)
        position = self.position(x)
        return action, position