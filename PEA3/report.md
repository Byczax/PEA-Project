# Programowanie Efektywnych Algorytmów - Projekt: Sprawozdanie

| Wydział Elektroniki      | Kierunek: Informatyka Techniczna |
| :----------------------- | -------------------------------: |
| Grupa zajęciowa Wt 17:05 |          Semestr: 2021/2022 Zima |
| Prowadzący:              |            Dr inż. Antoni Sterna |

|        Autor         |
| :------------------: |
| Byczko Maciej 252747 |

- [Programowanie Efektywnych Algorytmów - Projekt: Sprawozdanie](#programowanie-efektywnych-algorytmów---projekt-sprawozdanie)
  - [Wstęp teoretyczny](#wstęp-teoretyczny)
    - [Problem komiwojażera](#problem-komiwojażera)
    - [Algorytm genetyczny](#algorytm-genetyczny)
      - [Opis ogólny](#opis-ogólny)
      - [Wymagane parametry](#wymagane-parametry)
      - [Opis działania operatorów krzyżowania](#opis-działania-operatorów-krzyżowania)
      - [Opis sposobów mutacji](#opis-sposobów-mutacji)
      - [Etapy działania algorytmu](#etapy-działania-algorytmu)
  - [Opis zastosowanych klas w programie](#opis-zastosowanych-klas-w-programie)
  - [Plan eksperymentu](#plan-eksperymentu)
  - [Wyniki eksperymentów](#wyniki-eksperymentów)
  - [Wnioski](#wnioski)
  - [Bibliografia](#bibliografia)

## Wstęp teoretyczny

### Problem komiwojażera

Problem komiwojażera (ang. travelling salesman problem, TSP) – zagadnienie optymalizacyjne, polegające na znalezieniu drogi o najmniejszym koszcie.
komiwojażer - przedstawiciel firmy podróżujący w celu zdobywania klientów i przyjmowania zamówień na towar. (definicja ze słownika)
W celu zobrazowania problemu należy wyobrazić sobie tytułowego komiwojażera, który podróżuje między miastami w celu wykonywania swojej pracy. Podróż zaczyna z siedziby swojej firmy po czym jego trasa przebiega przez każde miasto dokładnie jeden raz, aż w końcu wraca z powrotem do głównego budynku firmy. Matematycznie prezentujemy ten problem jako graf którego wierzchołki są miastami a łączące je trasy to krawędzie z odpowiednimi wagami. Jest to pełny graf ważony oraz może być skierowany, co tworzy problem asymetryczny.
Rozwiązanie problemu komiwojażera sprowadza się do znalezienia właściwego - o najmniejszej sumie wag krawędzi - cyklu Hamiltona, czyli cyklu przechodzącego przez każdy wierzchołek grafu dokładnie jeden raz. Przeszukanie wszystkich cykli (czyli zastosowanie metody _Brute Force_(przegląd zupełny)) nie jest optymalną metodą, jako że prowadzi do wykładniczej złożoności obliczeniowej - $O(n!)$, dla której problemy o dużym $n$ traktowane jako nierozwiązywalne. Klasyfikuje to problem komiwojażera jako problem NP-trudny, czyli niedający rozwiązania w czasie wielomianowym. To powoduje konieczność skorzystania z tzw. algorytmów heurystycznych bądź metaheurystycznych (bardziej ogólnych), np. algorytmy _Tabu search_ bądź _Simulated annealing_. W naszym przypadku zagłębimy się w dziedzinę algorytmów ewolucyjnych, głównie _algorytm genetyczny_.

### Algorytm genetyczny

#### Opis ogólny

Algorytm genetyczny jest to rodzaj heurystyki, należy do grupy algorytmów ewolucyjnych, gdyż jego sposób działania jest zaczerpnięty z ewolucji biologicznej. Ewolucja rozpoczyna się od utworzenia początkowej populacji, stosowanie operatorów krzyżowania (rozmnażania) i mutacji (wpływ otoczenia na osobnika, np. wirus) tak aby dojść do rozwiązania jak najbliższego optymalnemu.

#### Wymagane parametry

- Graf, załadowany z pliku bądź wygenerowany losowo
- Czas działania algorytmu, podawany w sekundach
- Wielkość populacji
- Szansa na mutację
- Wybranie operatora krzyżowania:
  - Partially Matched Crossover (PMX)
  - Order Crossover (OX)
- Wybranie operatora mutacji:
  - Swap - zamiana dwóch elementów
  - Reverse - Zamiana kolejności elementów pomiędzy podanymi indeksami

#### Opis działania operatorów krzyżowania

<!-- TODO Opis -->

#### Opis sposobów mutacji

Przykład działania mutacji na przykładowej tablicy wartości:

```m
Tablica tab = [0,1,2,3,4,5]

Indeks a = 1
Indeks b = 4

Wynik mutacji Swap(tab, a, b) -> [0,4,2,3,1,5]
Wynik mutacji Reverse(tab, a, b) -> [0,4,3,2,1,5]
```

#### Etapy działania algorytmu

1. Na początku jest generowana losowa populacja $n$ osobników.
2. Ocena osobników - obliczenie ich kosztów
3. Selekcja rodziców
4. Tworzenie nowej populacji
5. Zastąpienie starej populacji jej nowym odpowiednikiem
6. Jeżeli nie przekroczono podanego czasu, przejdź do punktu 2.
7. Zwróć najlepsze rozwiązanie

Opis słownictwa:

- **Selekcja** - Wybór losowy z populacji 2 osobników, porównanie ich i zwracamy lepszego z nich i dodajemy do populacji rodziców. Powtarzamy dopóki liczba rodziców nie będzie identyczna co liczba osobników w populacji.
- **Tworzenie nowej populacji** - Pobranie z kandydatów 2 osobników, przeprowadzenie krzyżowania wybraną metodą oraz przeprowadzenie mutacji z zadaną szansą i zwrócenie osobników wynikowych

## Opis zastosowanych klas w programie

- **Program** - główny plik zawierający menu wraz z interfejsem użytkownika
- **Essentials** - Klasa statyczna z podstawowymi narzędziami jak np. liczenie wartości ścieżki, generowanie losowego problemu, etc.
- **Matrix** - Klasa statyczna reprezentująca problem jak macierz sąsiedztwa wraz z funkcjami na niej operującymi
- **Algorithm** - Klasa statyczna zawierająca wszystkie potrzebne algorytmy i funkcje które głównie działają przy algorytmie genetycznym: krzyżowanie, mutacje oraz sam algorytm.

## Plan eksperymentu

Program został napisany w języku `C#`, w `.NET` Framework, w środowisku `JetBrains Rider`.

Do mierzenia czasu wykorzystano klasę `StopWatch` z przestrzeni nazw `System.Diagnostics`.

Testowane instancje problemu były wielkości:
**17, 100** oraz **443**.

## Wyniki eksperymentów

## Wnioski

<!-- TODO Wnioski -->

## Bibliografia

<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"></script>
<script type="text/x-mathjax-config">MathJax.Hub.Config({ tex2jax: {inlineMath: [['$', '$']]}, messageStyle: "none" });</script>
