import os
import statistics
import matplotlib.pyplot as plt


def calc_ree(best, local):
    return (local - best)*100/best


def read_text_file(filepath):
    samples = 10
    results = {}
    x_array = []
    min_local = []
    min_global = []
    min_global_road = []
    with open(filepath, 'r') as txt_file:
        counter = 10
        lines = txt_file.readlines()
        best_road = int(lines[0])
        for line in lines[2:]:
            splitted = line.split(";")
            x_array.append(round(float(splitted[0])/10000, 2))
            min_local.append(calc_ree(best_road, int(splitted[1])))
            min_global.append(calc_ree(best_road, int(splitted[2])))
            min_global_road.append(int(splitted[2]))
            if int(splitted[2]) == int(lines[-1].split(";")[2]):
                counter -= 1
            if counter == 0:
                break
    os.makedirs("data", exist_ok=True)
    with open("data/" + filepath, 'a') as data_file:
        for i in range(len(x_array)):

            if int(min_global[i]) != int(min_global[i-1]):
                # print(str(round(x_array[i],2)) + " | " + str(min_global_road[i]) + " | " + str(round(min_global[i],2)))
                data_file.write(
                    f"| {round(x_array[i],2)} | {min_global_road[i]} | {round(min_global[i],2)} | \n")


def plot_single(x, y, title):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')

    plt.plot(x, y, marker='o', linestyle='-', color='limegreen',
             linewidth=2, markersize=10)

    plt.title(title.replace("-", " - "))
    plt.margins(x=None, y=None, tight=True)
    plt.ylabel("relative error [%]")
    plt.xlabel("time [ms]")
    plt.grid(True, color="lightgrey", alpha=0.5)

    os.makedirs("pictures", exist_ok=True)
    save_path = title.replace(" ", "") + ".png"
    plt.savefig("pictures/" + save_path)  # save plot to file
    print(title)


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
        for file in os.listdir():
            if file.endswith(".csv"):
                print(file)
                read_text_file(file)
        os.chdir("../")


def suppress_qt_warnings():
    os.environ["QT_DEVICE_PIXEL_RATIO"] = "0"
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    os.environ["QT_SCREEN_SCALE_FACTORS"] = "1"
    os.environ["QT_SCALE_FACTOR"] = "1"


if __name__ == '__main__':
    suppress_qt_warnings()
    main()
