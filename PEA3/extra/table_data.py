import os


def read_text_file(filepath):
    results_array = [["---"],["time [ms]"], ["best road"], ["relative error [%]"]]

    with open(filepath, 'r') as txt_file:
        lines = txt_file.readlines()
        iterator = 1
        if len(lines)>15:
            iterator = int(len(lines)/12)
        print(len(lines))
        for line in lines[::iterator]:
            print(line)
            time, road, relative = line.strip().split("|")
            # print(results_array)
            results_array[0].append("---")
            results_array[1].append(time)
            results_array[2].append(road)
            results_array[3].append(relative)
            # time_array.append(time)
            # road_array.append(road)
            # relative_array.append(relative)
        # time, road, relative = lines[-1].strip().split("|")
        # results_array[0].append("---")
        # results_array[1].append(time)
        # results_array[2].append(road)
        # results_array[3].append(relative)
    with open(f"results.csv", 'a') as data_file:
        data_file.write(f"\n#### {filepath.split('.')[0]}\n\n| ")
        
        for _ in range(len(results_array[0])-1):
            data_file.write(f"| ")
        data_file.write(f"|\n")
        for array in results_array:
            for element in array:
                data_file.write(f"|{element}")
            data_file.write(f"|\n")

        # data_file.write(f"{line}")
        # data_file.write(f"{lines[-1]}")


def create_table(files, title):
    pass


def main():
    path = "results"
    os.chdir(path)
    for folder in os.listdir():
        os.chdir(folder)
        os.chdir("data")
        previous = ""
        files = []
        for file in os.listdir():
            if file.endswith(".csv"):
                print(file)
                read_text_file(file)
        #         if previous == "":
        #             previous = int(file.split("-")[0])
        #     if file.endswith(".csv"):
        #         if previous == int(file.split("-")[0]):
        #             files.append(file)
        #             print(file)
        #         else:
        #             # print(files)
        #             # create_table(files)
        #             create_table(files, f"{folder}-{previous}")
        #             files = [file]
        #         # read_text_file(file, folder, previous)
        #         previous = int(file.split("-")[0])
        # create_table(files, f"{folder}-{previous}")
        # display_files(files, f"{folder}-{previous}")
                # read_text_file(file)
        os.chdir("../../")


if __name__ == '__main__':
    main()
