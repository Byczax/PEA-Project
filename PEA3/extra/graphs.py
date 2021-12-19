import os
import statistics
import matplotlib.pyplot as plt
from numpy import save


def calc_ree(best, local):
    return (local - best)*100/best


def read_text_file(filepath):
    x_array = []
    min_value = []
    relative = []
    # min_global = []
    # min_global_road = []
    with open(filepath, 'r') as txt_file:
        counter = 10
        lines = txt_file.readlines()
        best_road = int(lines[0])
        for line in lines[3:]:
            splitted = line.split(";")
            x_array.append(round(float(splitted[0])/10000, 2))
            # print(splitted)
            relative.append(calc_ree(best_road, int(splitted[1])))
            # min_global.append(calc_ree(best_road, int(splitted[2])))
            min_value.append(int(splitted[1]))
            if int(splitted[1]) == int(lines[-4].split(";")[1]):
                counter -= 1
            if counter == 0:
                break

    os.makedirs("data", exist_ok=True)

    # plot_single(x_array, relative, filepath)
    with open("data/" + filepath, 'a') as data_file:
        for i in range(len(x_array)):

            if int(min_value[i]) != int(min_value[i-1]):
                # print(str(round(x_array[i],2)) + " | " + str(min_global_road[i]) + " | " + str(round(min_global[i],2)))
                data_file.write(
                    f"{round(x_array[i],2)}|{round(min_value[i],2)}|{round(relative[i],2)}\n")
    return (x_array, relative)


def display_files(filenames, title):
    data = []
    for file in filenames:
        data.append([read_text_file(file), file])
        # print(data)
    plot_data(data, title)


def plot_data(data, title):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')
    # color_red = [   "pink", "orchid", "blueviolet", ]
    # color_lightblue = [, "aqua","navy"]
    # color_green = []
    # color_yellow = []
    # color_darkblue = []
    colors = ["crimson", "salmon", "red", "brown", "dodgerblue", "blue", "steelblue", "lightblue", "limegreen",
              "olive", "lawngreen", "seagreen", "tan", "orange", "gold", "moccasin", "violet", "fuchsia", "magenta", "orchid"]
    color_counter = 0
    for values, subtitle in data:
        # print(subtitle)
        if title.split("-")[0] == "cross":
            subtitle = "".join(
                [f"{fragment} " for fragment in subtitle.split("-")[3:]])
        elif title.split("-")[0] == "mut":
            subtitle = "".join(
                [f"{fragment} " for fragment in subtitle.split("-")[2:]])
            subtitle = subtitle.replace("0_8", "")
        elif title.split("-")[0] == "pop":
            subtitle = "".join(
                [f"{fragment} " for fragment in subtitle.split("-")[1:]])
            subtitle = subtitle.replace("0_01 0_8", "")

        subtitle = subtitle.strip().replace("_", ",")
        # subtitle = subtitle.replace("_",".")

        #     subtitle = subtitle.split("-")[3:]
    # x, y = 0
        x, y = values
        plt.plot(x, y, marker='o', linestyle='-', color=colors[color_counter],
                 linewidth=2, markersize=1, label=subtitle.split(".")[0])
        color_counter += 1

    plt.legend(loc="best")
    plt.title(title.replace("-", " - "))
    plt.margins(x=None, y=None, tight=True)
    plt.ylabel("relative error [%]")
    plt.xlabel("time [ms]")
    plt.grid(True, color="lightgrey", alpha=0.5)
    plt.yscale('log')
    plt.xscale('log')
    # plt.legend(loc='center left', bbox_to_anchor=(1, 0.5))
    os.makedirs("pictures", exist_ok=True)
    save_path = title.replace(" ", "") + ".png"
    plt.savefig("pictures/" + save_path)  # save plot to file
    print(save_path)


def plot_single(x, y, title):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')

    plt.plot(x, y, marker='o', linestyle='-', color='limegreen',
             linewidth=2, markersize=1, label="global minimum")

    plt.title(title.replace("-", " - "))
    plt.margins(x=None, y=None, tight=True)
    plt.ylabel("relative error [%]")
    plt.xlabel("time [ms]")
    plt.grid(True, color="lightgrey", alpha=0.5)

    os.makedirs("pictures", exist_ok=True)
    save_path = title.replace(" ", "") + ".png"
    plt.savefig("pictures/" + save_path)  # save plot to file
    # print(title)


def plot_double(x, y, y2, title):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')

    plt.plot(x, y, marker='o', linestyle='-', color='lightgray',
             linewidth=2, markersize=1, label="local minimum")
    plt.plot(x, y2, marker='o', linestyle='-', color='limegreen',
             linewidth=2, markersize=1, label="global minimum")

    plt.title(title.replace("-", " - "))
    plt.margins(x=None, y=None, tight=True)
    plt.legend(loc="best")
    plt.ylabel("relative error [%]")
    plt.xlabel("time [ms]")
    plt.grid(True, color="lightgrey", alpha=0.5)
    # plt.locator_params(nbins=10)
    os.makedirs("pictures", exist_ok=True)
    save_path = title.replace(" ", "") + ".png"
    plt.savefig("pictures/" + save_path)  # save plot to file
    print(title)


def main():
    path = "results"
    os.chdir(path)
    for folder in os.listdir():
        os.chdir(folder)
        previous = ""
        files = []
        for file in os.listdir():
            # print(file)
            # print(previous)

            if previous == "":
                previous = int(file.split("-")[0])
            if file.endswith(".csv"):
                if previous == int(file.split("-")[0]):
                    files.append(file)
                    print(file)
                else:
                    # print(files)
                    display_files(files, f"{folder}-{previous}")
                    files = [file]
                # read_text_file(file, folder, previous)
                previous = int(file.split("-")[0])
        display_files(files, f"{folder}-{previous}")
        # files = []
        os.chdir("../")


def suppress_qt_warnings():
    os.environ["QT_DEVICE_PIXEL_RATIO"] = "0"
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    os.environ["QT_SCREEN_SCALE_FACTORS"] = "1"
    os.environ["QT_SCALE_FACTOR"] = "1"


if __name__ == '__main__':
    suppress_qt_warnings()
    main()
