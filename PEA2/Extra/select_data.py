import os


def read_text_file(filepath):
    with open(filepath, 'r') as txt_file:
        with open("results.csv", 'a') as data_file:
            lines = txt_file.readlines()
            print(len(lines))
            data_file.write(f"{filepath}\n")
            for line in lines[::int(len(lines)/20)]:
                data_file.write(f"{line}")


def main():
    path = "results/443/data"
    os.chdir(path)
    # for folder in os.listdir():
    # os.chdir(folder)
    for file in os.listdir():
        if file.endswith(".csv"):
            print(file)
            read_text_file(file)
        # os.chdir("../")


if __name__ == '__main__':
    main()
