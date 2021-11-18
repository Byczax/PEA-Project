import matplotlib.pyplot as plt
import os
import statistics
import numpy as np
import math
results = []


def read_text_file(filepath):
    with open(filepath, 'r') as txt_file:
        counter = 0
        round_values = []
        road_time = []
        for line in txt_file:
            counter += 1
            splitted = line.split(";")
            road_time.append(float(splitted[1]))
            if counter == 100:
                # time_list = [round(q, 1) for q in statistics.quantiles(road_time, n=10)]
                time_mean = statistics.mean(road_time)

                round_values.append([int(splitted[0]), time_mean])
                road_time.clear()
                counter = 0
    results.append([filepath.split(".")[0], round_values])


def plot_single(data, title):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')

    # x = np.linspace(3,13,10)

    # y = []
    # for _ in range(3,13):
    #     y.append(((x**2)*(2**x))/100000)

    result = calculate_data_min(data[1])

    plt.plot(result[0], result[1], marker='o', linestyle='-', color='limegreen',
             linewidth=2, markersize=10)
    # plt.plot(x, y, marker='o', linestyle='-.', color='blue',
    #          linewidth=2, markersize=1, label=data[1][0])

    plt.title(title)
    plt.margins(x=None, y=None, tight=True)
    # plt.legend(loc="upper left")
    plt.ylabel("Time [ms]")
    plt.xlabel("Element quantity")
    plt.grid(True, color="lightgrey", alpha=0.5)
    os.chdir("../pictures")
    save_path = title + ".png"
    plt.savefig(save_path)  # save plot to file


def draw_graph(data):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')

    # plt.plot(data_x, data_y, marker='o', linestyle='-', color='limegreen',
    #          linewidth=2, markersize=10, label=data[0])
    # print(data[0][1])
    result = calculate_data_min(data[0][1])

    plt.plot(result[0], result[1], marker='o', linestyle='-', color='green',
             linewidth=2, markersize=10, label=data[0][0])

    result = calculate_data_min(data[1][1])

    plt.plot(result[0], result[1], marker='o', linestyle='-', color='blue',
             linewidth=2, markersize=10, label=data[1][0])

    result = calculate_data_min(data[2][1])

    plt.plot(result[0], result[1], marker='o', linestyle='-', color='red',
             linewidth=2, markersize=10, label=data[2][0])

    result = calculate_data_min(data[3][1])

    plt.plot(result[0], result[1], marker='o', linestyle='-', color='orange',
             linewidth=2, markersize=10, label=data[3][0])

    # result = calculate_data(data[1])
    # plot_data(result, data[1][0], 1)

    # result = calculate_data(data[2])
    # plot_data(result, data[2][0], 0)

    # result = calculate_data(data[3])
    # plot_data(result, data[3][0], 1)

    # result = calculate_data(data[1][1], 3)
    # plot_data(result, first_name, 0)

    # result = calculate_data(data[1][1], select)
    # plot_data(result, second_name, 1)

    # result = calculate_data(data[0][1], 3)
    # plot_data(result, second_name, 1)

    plt.title("Czasy rozwiązania problemów")
    plt.margins(x=None, y=None, tight=True)
    plt.legend(loc="upper left")
    plt.ylabel("Time [ms]")
    plt.xlabel("Element quantity")
    plt.grid(True, color="lightgrey", alpha=0.5)
    os.chdir("../pictures")
    save_path = "Czasy_duze" + ".png"
    plt.savefig(save_path)  # save plot to file
    # plt.show()  # show plots in IDE


def calculate_data_min(data):
    data_x = []
    data_y = []
    print("==============")
    for item in data:
        print(str(item[0]) + "|" + str(item[1]))
        # if item[0] < 10:
        #     continue
        data_x.append(item[0])
        data_y.append(item[1])
    return [data_x, data_y]


# def plot_data(data, name, pack_number):
#     color_packs = [["gray", "maroon", "red", "silver"], ["darkblue", "sienna", "teal", "crimson"]]
#     add_to_plot(data[1], data[0], color_packs[pack_number][0], name)

# def add_to_plot(data_x, data_y, color, name):
#     plt.plot(data_x, data_y, marker='o', linestyle='-', color=color,
#              linewidth=2, markersize=10, label=name)


def main():
    path = "results"
    os.chdir(path)
    for file in os.listdir():
        if file.endswith(".csv"):
            print(file)
            read_text_file(file)

    plot_single(results[0], "Branch & Bound - Breath Search")
    plot_single(results[1],"Branch & Bound - Deep Search")
    plot_single(results[2],"Brute Force")
    plot_single(results[3],"Dynamic Programmming")

    draw_graph(results)


def suppress_qt_warnings():
    os.environ["QT_DEVICE_PIXEL_RATIO"] = "0"
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    os.environ["QT_SCREEN_SCALE_FACTORS"] = "1"
    os.environ["QT_SCALE_FACTOR"] = "1"


if __name__ == '__main__':
    suppress_qt_warnings()
    main()
