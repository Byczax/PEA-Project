import os
import enum
import matplotlib.pyplot as plt


class Algorithm(enum.Enum):
    SimulatedAnnealing = 0
    TabuSearch = 1


class Neighbour(enum.Enum):
    SWAP = 0
    REVERSE = 1


class Diversification(enum.Enum):
    OFF = 0
    ON = 1


def read_text_file(filepath):
    counter = 0
    sum = 0
    results = [[[], []], [[[], []], [[], []]]]
    with open(filepath, 'r') as txt_file:
        for line in txt_file:
            counter += 1
            splitted = line.split(";")
            sum += int(splitted[5].split(":")[1])
            if counter == 5:
                sum /= 5
                if splitted[0] == "Algorithm:SimulatedAnnealing":  # Annealing
                    if splitted[2] == "Neighbourhood:SWAP":  # Annealing | swap
                        results[0][0].append([int(splitted[1].split(":")[1]), int(splitted[4].split(
                            ":")[1]), sum, int(splitted[6].split(":")[1])])
                    else:  # Annealing | reverse
                        results[0][1].append([int(splitted[1].split(":")[1]), int(splitted[4].split(
                            ":")[1]), sum, int(splitted[6].split(":")[1])])
                else:  # Tabu
                    if splitted[2] == "Neighbourhood:SWAP":  # Tabu | swap
                        if splitted[3] == "Diversification:OFF":  # Tabu | swap | divOFF
                            results[1][0][0].append([int(splitted[1].split(":")[1]), int(splitted[4].split(
                                ":")[1]), sum, int(splitted[6].split(":")[1])])
                        else:  # Tabu | swap | divON
                            results[1][0][1].append([int(splitted[1].split(":")[1]), int(splitted[4].split(
                                ":")[1]), sum, int(splitted[6].split(":")[1])])

                    else:  # Tabu | reverse
                        if splitted[3] == "Diversification:OFF":  # Tabu | reverse | divOFF
                            results[1][1][0].append([int(splitted[1].split(":")[1]), int(splitted[4].split(
                                ":")[1]), sum, int(splitted[6].split(":")[1])])
                        else:  # Tabu | reverse | divON
                            results[1][1][1].append([int(splitted[1].split(":")[1]), int(splitted[4].split(
                                ":")[1]), sum, int(splitted[6].split(":")[1])])
                counter = 0
                sum = 0
    return results


def process_data(data, title):
    counter = 0
    x_array = []
    y_array = []
    for element in data:
        counter += 1
        x_array.append(int(element[0]))
        y_array.append((int(element[2])-int(element[3]))
                       * 100/(int(element[3])*1.0))
        if counter == 5:
            plot_single(x_array, y_array, title + str(element[1]))
            for i in range(5):
                print(str(x_array[i]) + "|" + str(round(y_array[i], 3)))
            counter = 0
            x_array = []
            y_array = []


def plot_single(x, y, title):
    plt.rcParams.update({'font.size': 20})
    plt.figure(num=None, figsize=(20, 8), dpi=400,
               facecolor='w', edgecolor='k')

    plt.plot(x, y, marker='o', linestyle='-', color='limegreen',
             linewidth=2, markersize=10)

    plt.title(title)
    plt.margins(x=None, y=None, tight=True)
    plt.ylabel("relative error [%]")
    plt.xlabel("Element quantity")
    plt.grid(True, color="lightgrey", alpha=0.5)
    # os.chdir()

    os.makedirs("pictures", exist_ok=True)
    save_path = title.replace(" ", "") + ".png"
    plt.savefig("pictures/" + save_path)  # save plot to file
    print(title)
    # print("abc")


def main():
    # path = "results"
    # os.chdir(path)
    for file in os.listdir():
        if file.endswith(".csv"):
            print(file)
            results = read_text_file(file)
            process_data(results[0][0], "Simulated Annealing - SWAP - ")
            process_data(results[0][1], "Simulated Annealing - REVERSE - ")
            process_data(results[1][0][0],
                         "Tabu Search - SWAP - Dywersyfikacja OFF - ")
            process_data(results[1][0][1],
                         "Tabu Search - SWAP - Dywersyfikacja ON - ")
            process_data(
                results[1][1][0], "Tabu Search - REVERSE - Dywersyfikacja OFF - ")
            process_data(results[1][1][1],
                         "Tabu Search - REVERSE - Dywersyfikacja ON - ")
            # tabu_search = process_data(results[1])
            # process_annealing(results[0])
            # print(results)


def suppress_qt_warnings():
    os.environ["QT_DEVICE_PIXEL_RATIO"] = "0"
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    os.environ["QT_SCREEN_SCALE_FACTORS"] = "1"
    os.environ["QT_SCALE_FACTOR"] = "1"


if __name__ == '__main__':
    suppress_qt_warnings()
    main()
