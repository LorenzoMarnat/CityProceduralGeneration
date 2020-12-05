# Génération procédurale de villes

Le projet a été effectué en C# sous Unity.

Le but de ce projet est de générer une ville de manière procédurale à partir de seulement cinq _assets_. Pour cela, nous utiliserons plusieurs méthodes: placement aléatoire des bâtiments
, placement en fonction de la distance par rapport au centre-ville ainsi que le placement d'après du bruit de Perlin.
Nous essayerons les différentes approches afin de déterminer laquelle semble donner les meilleurs résultats et étudieront les solutions proposées par d'autres personnes.

## Sommaire

1. Placement aléatoire
2. Distance au centre
3. Bruit de Perlin
4. Conclusion

## Placement aléatoire

Avant de générer procéduralement une ville, il faudrait se demander quel schéma suivent les bâtiments au sein de cette ville. Mais ici, nous allons suivre une démarche inverse,
plus empirique. En effet, nous commencerons par placer nos cinq assets de manière totalement aléatoire pour ensuite étudier quels sont les défauts de cette approche et comment nous pourrions
obtenir un placement plus adéquat.

### Les assets

Tout d'abord, voici les cinq assets:

![assets](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/assets.PNG)

Avec de gauche à droite:
1. Herbe
2. Pavillon
3. Petit immeuble
4. Grand immeuble
5. Gratte-ciel

Le premier asset servira à représenter les espaces verts (parcs, bois, jardins...) et les quatre autres les différentes tailles de bâtiments.

Ces assets seront placés selon une grille carré représentant la ville. La grille a une taille variable selon le paramètre _gridSize_ entré par l'utilisateur.
Chacune des cases possède un unique asset parmi la liste disponible

### Tirage aléatoire

Comme indiqué précédemment, dans cette première approche chaque case a un asset choisi aléatoirement. Pour cela, on tire un entier entre 1 et 5. L'entier 1 correspond à l'herbe,
les entiers de 2 à 5 correspondent respectivement à un bâtiment.

Voici le résultat obtenu:

![random](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/random.PNG)

### Conclusion

Le résultat de cette première approche peut faire illusion pour des petits bouts de villes. L'image obtenue n'est d'ailleurs pas sans nous rappeler certains centres-villes de grandes métropoles, par exemple ici New York.

<img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/random.PNG" alt="random" width="500"/> <img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/newyork.jpg" alt="newYork" width="500"/>

Cette solution peut paraitre efficace pour représenter des zones denses en immeubles et gratte-ciels, où les tailles sont extrêmement variables d'un bâtiment à son voisin.
Néanmoins, elle est beaucoup moins satisfaisante quand il s'agit de générer une ville entière.

## Distance au centre

Dans cette deuxième approche, nous allons essayer d'obtenir une ville plus cohérente est ajoutant un paramètre dans le choix du bâtiment: la distance de ce dernier par rapport
au centre-ville. En effet, en étudiant les photos de plusieurs grandes villes, on remarque qu'un même schéma semble se répéter: les plus grands bâtiments sont concentrés sur une même zone.

Par exemple, voici une vue de Melbourne:
![melbourne](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/melbourne.jpg)

On observe que les gratte-ciels se trouvent tous proches du centre de Melbourne. Quand on s'éloigne du centre, les bâtiments perdent rapidement en taille. Nous avons d'abord des immeubles, puis des pavillons sur seulement un ou deux niveaux.

### Choix selon la distance

Cette fois-ci, à la place de choisir aléatoirement l'asset de chaque case, nous allons choisir le choisir en fonction de la distance entre la case et le centre-ville.
Le centre-ville est déterminé automatiquement comme étant le centre de la grille. Chaque asset correspond à l'un des cinq cercles concentriques autour de ce centre. Les cercles sont déterminés par cet algorithme:

```
distance = Distance(building(i,j),townCenter)
radius = gridSize / 2

if (distance < 0.33 * radius) then return 4    # Premier cercle, correspond à l'asset gratte-ciel
elif (distance < 0.66 * radius) then return 3  # Deuxième cercle, correspond à l'asset grand immeuble
elif (distance < radius) then return 2         # Troisième cercle, correspond à l'asset petit immeuble
elif (distance < 1.2 * radius) then return 1   # Quatrième cercle, correspond à l'asset pavillon
else return 0                                  # Cinquième cercle, correspond à l'asset herbe
```

Voici le résultat obtenu:

![distance](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/distance.PNG)

Il y a deux problèmes évidents qui sautent aux yeux:
- Déjà, le résultat est beaucoup trop uniforme
- De plus, cela empêche totalement d'avoir par exemple de la verdure en centre-ville ou un immeuble en bordure

Nous allons essayer de corriger ces problèmes en fusionnant cette méthode avec l'approche aléatoire.

### Distance au centre et aléatoire

Afin de pallier l'uniformité des cercles concentriques, nous allons non plus assigner à chacun un seul asset mais une liste de trois assets. Ainsi, pour chaque case, nous tirons un asset aléatoire dans une des listes réduites en fonction de la distance entre la case et le centre de la grille. Les listes des cercles proches du centre contiennent les plus grands bâtiments, inversement les cercles de bordures n'ont que l'herbe et le pavillon comme assets dans leur liste respective.
Les listes sont les suivantes:
- Premier cercle -> gratte-ciel, grand immeuble, herbe
- Deuxième cercle -> grand immeuble, petit immeuble, herbe
- Troisième cercle -> petit immeuble, pavillon, herbe
- Quatrième cercle -> pavillon, pavillon, herbe
- Cinquième cercle -> pavillon, herbe, herbe

La répétition d'un même asset dans les deux dernières listes permet d'augmenter la probabilité d'apparition de l'un ou l'autre.

Voici le résultat obtenu avec ces nouvelles listes:

![random+distance](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/random%2Bdistance2.PNG)

Le résultat est bien meilleur, notamment grâce à la variance de taille entre les bâtiments voisins. La ville est moins uniforme et les cercles apparaissent de manière moins évidente. De plus, la possibilité de jouer avec les listes d'assets de chaque cercle et les taux d'apparitions de chaque asset offre une bonne flexibilité à cette solution. Malheureusement, cette approche pèche pour la création de _blocs_ d'un même asset. Par exemple, elle ne permet pas de créer un espace vert de grande taille ou de représenter un lotissement.

