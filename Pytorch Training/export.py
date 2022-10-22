import argparse
import torch
from model import HandModel

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Export checkpoints into an ONNX model")
    parser.add_argument('filename', type=str, help='Checkpoint file to export')

    args = parser.parse_args()

    filename = args.filename
    if not filename.endswith(".pt"):
        filename += ".pt"
    
    output = filename.replace(".pt", ".onnx")

    model = HandModel()
    checkpoint = torch.load(filename)
    model.load_state_dict(checkpoint['model_state_dict'])

    dummy_image = torch.zeros((1, 3, 120, 160))
    dummy_prev_action = torch.zeros((1))
    dummy_prev_position = torch.zeros((1, 3))
    dummy_pred_action = torch.zeros((1, 4))
    dummy_pred_position = torch.zeros((1, 3))

    torch.onnx.export(
        model,
        (dummy_image, dummy_prev_action, dummy_prev_position),
        output,
        input_names = ["image", "prev_action", "prev_position"],
        output_names = ["pred_action", "pred_position"],
        export_params = True,
        opset_version = 9,
        do_constant_folding = True,
    )