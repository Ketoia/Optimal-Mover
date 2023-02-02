# Projekt_Zespolowy_Dobry
Robili:
- Jakub Gromadzki
- Marek Gromadzki
- Michał Chojnowski

Projekt ten to prosta gra która polega na koszeniu trawy po torusie.

![image](https://user-images.githubusercontent.com/48310919/216402400-cb0cc392-b67e-4cd8-ba02-9541cb89e21f.png)

# Movement
Poruszanie zostało napisane w skrypcie "MovementStateManager.cs". Polega ono na użyciu komponentu z silnika (rigidbody), i dodaniu do niego siły w zależności od kierunku patrzenia się kosiarki (żeby ruszała się tylko przed siebie). Rotacja kosiarki została rozdzielona na dwie częsci, pierwsza Kosiarka obraca się wokół własnej osi Y, zostało to zrobione przy użyciu funkcji rotate i zostało wykorzystane do kierowania kosiarką. Druga część rotacji polega na dostosowaniu rotacji tak aby kosiarka była równoległa do płaszczyzny na której stoi. Zrobiliśmy to używając raycastów z kilku punktów kosiarki, potem jeśli te raycasty trafiły w jakiś punkt, z tego punktu obliczana jest rotacja x i z.

# Grawitacja
Grawitacja polega na odnalezieniu najbliższego środka torusu, potem ciągłe dodawanie odwrotności tego kierunku do ogólnego kierunku poruszania się kosiarki.

# Koszenie trawy
Koszenie trawy zostało napisane w skrypcie "GrassGenerator.cs". Najpierw znajduje punkty gdzie ma być instancjonowana trawa. Użyliśmy do tego wierzchołków modelu torusa. Potem obliczyliczyśmy jego rotacje oraz punkt punkt koordynatów UV na tym torusie, do odnalezienia tego punktu użyliśmy funkcji Raycast. Te 3 wartości (pozycja, rotacja, pozycja na UV), wrzucamy do compute Shadera (GrassComputeShader.compute), gdzie koszenie trawy będzie obliczane. Po ogbliczeniu wszystkich potrzebnych rzeczy, gotową listę obiektów wrzucamy do GPUInstancera, z dwoma wartościami (pozycją i rotacją). W vertex shaderze trawy którą isntancjonujemy ustawiana jest pozycja oraz rotacja, a w fragment shaderze ustawiane są kolory (cały shader znajduje się w pliku "GrassShader.shader").
