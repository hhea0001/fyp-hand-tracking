import argparse

import torch
import torch.optim as optim
import torch.nn.functional as F
from torch.utils.data import DataLoader

from model import HandModel
from data import HandDataset

GPU_indx = 0
device = torch.device(GPU_indx if torch.cuda.is_available() else 'cpu')

def train(model: HandModel, type, output, epochs: int, group_size: int, learning_rate: float, train_dataloader, valid_dataloader):
    optimiser = optim.Adam(model.position.parameters(), learning_rate)
    best_train_loss = 1000000
    best_valid_loss = 1000000

    for epoch in range(1, epochs + 1):

        model_dict = model.state_dict()

        print(f"--- Starting epoch {epoch}")
        #train_loss = train_epoch(model, type, group_size, optimiser, train_dataloader)
        #print(f"Average train loss: {train_loss}")

        if valid_dataloader == None:
            if train_loss < best_train_loss:
                best_train_loss = train_loss
                print(f"--- Best training loss so far. Saving to {output}")
                torch.save({
                    'model_state_dict': model.state_dict(),
                    'best_train_loss': best_train_loss,
                    'best_valid_los': best_valid_loss
                }, output)
        
        else:
            valid_loss = valid_epoch(model, type, group_size, valid_dataloader, True)
            print(f"Average valid loss: {valid_loss}")

            if valid_loss < best_valid_loss:
                best_valid_loss = valid_loss
                print(f"--- Best validation loss so far. Saving to {output}")
                torch.save({
                    'model_state_dict': model.state_dict(),
                    'best_train_loss': best_train_loss,
                    'best_valid_los': best_valid_loss
                }, output)
            else:
                model.load_state_dict(model_dict)



        # if valid_dataloader != None:
        #     print("--- Running validation...")
        #     valid_loss = test_epoch(type, model, valid_dataloader, 50, True)
        #     print(f"--- End of validation {epoch} \t Average Loss: {valid_loss}")

def train_epoch(model: HandModel, type, group_size, optimiser: optim.Adam, dataloader):
    model.train()
    count = 0
    loss_sum = 0

    for i, (image, action, position, prev_action, prev_position) in enumerate(dataloader):

        if i >= group_size:
            break
        
        optimiser.zero_grad()

        pred_action, pred_position = model(image.to(device), prev_action.to(device), prev_position.to(device))
        action, position = action.to(device), position.to(device)

        pred_position[action == 0] = position[action == 0]

        loss_action = F.cross_entropy(pred_action, action)
        loss_position = F.mse_loss(pred_position, position)

        if type == 0:
            loss = loss_action
        elif type == 1:
            loss = loss_position
        else:
            loss = loss_action + loss_position
        
        predicted = torch.argmax(pred_action, dim=1)
        action_correct = (predicted == action).sum().item()
        total_action = action.size(0)

        loss_sum += loss.item()
        count += 1

        loss.backward()
        optimiser.step()

        if i % 20 == 0:
            print(f"Training ({i}/{group_size}) \t Action Accuracy: {action_correct/total_action * 100:.1f}% \t Action Loss: {loss_action.item():.6f} \t Position Loss: {loss_position.item():.6f}")
    
    return loss_sum / count

def valid_epoch(model: HandModel, type, group_size, dataloader, ignore_depth = False):
    model.eval()
    loss_sum = 0
    count = 0

    correct = 0
    total = 0

    with torch.no_grad():
        for i, (image, action, position, prev_action, prev_position) in enumerate(dataloader):

            if i >= 500:
                break

            pred_action, pred_position = model(image.to(device), prev_action.to(device), prev_position.to(device))
            action, position = action.to(device), position.to(device)

            pred_position[action == 0] = position[action == 0]

            if ignore_depth:
                pred_position[:, 2] = position[:, 2]

            loss_action = F.cross_entropy(pred_action, action)
            loss_position = F.mse_loss(pred_position, position)

            if type == 0:
                loss = loss_action
            elif type == 1:
                loss = loss_position
            else:
                loss = loss_action + loss_position

            predicted = torch.argmax(pred_action, dim=1)
            action_correct = (predicted == action).sum().item()
            position_correct = (((pred_position[action != 0][:, 0] - position[action != 0][:, 0])**2 + (pred_position[action != 0][:, 1] - position[action != 0][:, 1])**2) < 0.01).sum().item()

            total_action = action.size(0)
            total += total_action
            correct += position_correct

            loss_sum += loss.item()
            count += 1

            if i % 20 == 0:
                print(f"Validating ({i}/{500}) \t Action Accuracy: {action_correct/total_action * 100:.1f}% \t Position Accuracy: {correct/total * 100:.1f}% \t Action Loss: {loss_action.item():.6f} \t Position Loss: {loss_position.item():.6f}")

    return loss_sum / count

# def test_epoch(type: int, model: HandModel, dataloader: DataLoader, print_every: int = 0, ignore_depth = False):
#     model.eval()

#     loss_sum = 0
#     action_loss_sum = 0
#     position_loss_sum = 0
#     count = 0
    
#     total_action_correct = 0
#     total_action = 0

#     with torch.no_grad():
#         for i, (image, action, position, prev_action, prev_position) in enumerate(dataloader):

#             pred_action, pred_position = model(image.to(device), prev_action.to(device), prev_position.to(device))
#             action, position = action.to(device), position.to(device)

#             pred_position[action == 0] = position[action == 0]

#             if ignore_depth:
#                 pred_position[:, 2] = position[:, 2]

#             loss_action = F.cross_entropy(pred_action, action)
#             loss_position = F.mse_loss(pred_position, position)

#             if type == 0:
#                 loss = loss_action
#             elif type == 1:
#                 loss = loss_position
#             else:
#                 loss = loss_action + loss_position

#             predicted = torch.argmax(pred_action, dim=1)
#             total_action_correct += (predicted == action).sum().item()
#             total_action += action.size(0)

#             action_loss_sum += loss_action.item()
#             position_loss_sum += loss_position.item()
#             loss_sum += loss.item()
#             count += 1

#             if print_every > 0:
#                 if i % print_every == 0:
#                     print(f"Testing ({i}/{len(dataloader)}) \t Action Accuracy: {total_action_correct/total_action * 100:.1f}% \t Action Loss: {action_loss_sum/count:.6f} \t Position Loss: {position_loss_sum/count:.6f}")

#     return loss_sum / count

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Training the hand tracking model")
    parser.add_argument('--train', '-t', type=str, nargs='+', required=True, help='Dataset directory to use for training')
    parser.add_argument('--filter', '-f', type=int, nargs='+', default=[0, 1, 2, 3], help='Filter datasets by specific action (0, 1, 2, 3) -> (Not In Frame, None, Pinch, Point)')
    parser.add_argument('--type', type=str, nargs='+', choices=["action", "position"], default=["action", "position"], help="Type of model to train")
    parser.add_argument('--valid', '-v', type=str, nargs='+', help='Dataset directory to use for validation')
    parser.add_argument('--output', '-o', type=str, required=True, help='Filename to save checkpoints to')
    parser.add_argument('--epochs', '-e', type=int, required=True, help='Number of epochs to run')
    parser.add_argument('--checkpoint', '-c', type=str, help='Checkpoint file to load to start training')
    parser.add_argument('--learningrate', '-lr', type=float, default=3e-4, help="Learning rate to train")
    parser.add_argument('--batch', '-b', type=int, default=10, help="Batch size")
    parser.add_argument('--limit', '-l', type=int, default=None, help="Limit the training to an amount of images")
    parser.add_argument('--group', '-g', type=int, default=2000, help="Group size")

    args = parser.parse_args()

    model = HandModel().to(device)

    train_dataset = HandDataset(dirs = args.train, training = True, filter = args.filter, limit = args.limit)
    train_dataloader = DataLoader(dataset = train_dataset, batch_size = args.batch, shuffle = True)

    if args.valid:
        valid_dataset = HandDataset(dirs = args.valid, training = False, filter = args.filter)
        valid_dataloader = DataLoader(dataset = valid_dataset, batch_size = args.batch, shuffle = True)
    else:
        valid_dataset = None
        valid_dataloader = None

    type = 1
    if "action" in args.type:
        if "position" in args.type:
            type = 2
        else:
            type = 0
    
    output = args.output
    if not output.endswith(".pt"):
        output += ".pt"

    if args.checkpoint:
        checkpoint = args.checkpoint
        if not checkpoint.endswith(".pt"):
            checkpoint += ".pt"
        check_point = torch.load(checkpoint)
        model.load_state_dict(check_point['model_state_dict'])
    
    train(model = model, type = type, epochs = args.epochs, group_size=args.group, learning_rate = args.learningrate, output = output, train_dataloader = train_dataloader, valid_dataloader = valid_dataloader)
