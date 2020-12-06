# Génération procédurale de villes

  Le projet a été effectué en C# sous Unity. Les différentes méthodes de génération peuvent être essayées en variant l'attribut _pickMod_ (de 0 à 4) de l'objet _Grid_ de la scène. L'attribut _buildingSize_ de ce même objet permet de varier la distance entre les bâtiments et ainsi de réduire/augmenter la densité de la ville.

  Le but de ce projet est de générer une ville de manière procédurale à partir de seulement cinq _assets_. Pour cela, nous utiliserons plusieurs méthodes: placement aléatoire des bâtiments, placement en fonction de la distance par rapport au centre-ville ainsi que le placement d'après du bruit de Perlin.
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

if (distance < 0.33 * radius) return 4       # Premier cercle, correspond à l'asset gratte-ciel
else if (distance < 0.66 * radius) return 3  # Deuxième cercle, correspond à l'asset grand immeuble
else if (distance < radius) return 2         # Troisième cercle, correspond à l'asset petit immeuble
else if (distance < 1.2 * radius) return 1   # Quatrième cercle, correspond à l'asset pavillon
else return 0                                # Cinquième cercle, correspond à l'asset herbe
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

  Le résultat est bien meilleur, notamment grâce à la variance de taille entre les bâtiments voisins. La ville est moins uniforme et les cercles apparaissent de manière moins évidente. De plus, la possibilité de jouer avec les listes d'assets de chaque cercle et les taux d'apparitions de chaque asset offre une bonne flexibilité à cette solution. 

  Malheureusement, cette approche pèche pour la création de _blocs_ d'un même asset. Par exemple, elle ne permet pas de créer un espace vert de grande taille ou de représenter un lotissement car il y a rarement des zones avec un seul et même asset.

## Bruit de Perlin

  Afin d'obtenir des zones plus cohérentes, avec par exemple un seul asset dans toute une zone, cette nouvelle approche utilise le bruit de Perlin. En traduisant le bruit de Perlin en entiers entre 1 et 5, on peut générer une ville avec un rendu moins "aléatoire" que les approches précédentes. Cette solution s'inspire de la vidéo "Generating a Procedural City with Unity 5 Part 1" de _Holistic3d_.

<p>
  <img align="right" alt="perlinNoise2D" width="200" src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/PerlinNoise2d.png">
  Ainsi, les zones les plus sombres du bruit correspondront au plus haut bâtiment, le gratte-ciel. Inversement, les zones les plus claires joueront le rôle de l'herbe. Les autres bâtiments seront représentés par les nuances de gris, de plus en plus hauts quand la nuance tire vers le noir. De plus, l'introduction d'une <i>seed</i>, aléatoire ou arbitraire, permettra de varier les résultats. </p><br><br><br>
  
### Bruit seul

  Ici, on se contente de choisir un asset parmi les cinq disponibles en fonction de la valeur du bruit.

![noise](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/noise.PNG)

  Le rendu est très intéressant, notamment pour la création d'espaces verts: des parcs de grandes tailles au sein de la ville, proches que ce qu'on peut espérer trouver dans de grandes villes (des grands parcs plutôt qu'un multitude de minuscules espaces verts). Mais tout comme l'approche totalement aléatoire, on aimerait se rapprocher d'une image plus réaliste d'une ville en jouant sur la taille des bâtiments en fonction de leur distance au centre-ville.

### Bruit et distance au centre

  Cette fois, on reprend le système de cercles concentriques autour du centre-ville. Ainsi, comme précédemment, on ne choisi plus un asset parmi les cinq disponibles mais parmi une liste réduite en fonction de la distance avec le centre-ville. Les cinq cercles restent les mêmes (voir partie _Distance au centre_).

  Pour chaque case de la grille, on tire une valeur de bruit, nommée _noise_, en fonction de la position _ij_ de la case. Cette valeur est montée entre 0 et 100 afin de pouvoir jouer avec les taux d'apparitions d'un asset.

```
int noise = (int)(PerlinNoise(i + seed, j + seed) * 100);
```

Un asset correspond à une fourchette de bruit. Les assets choisis et les fourchettes varient d'un cercle à l'autre.

```
# 4: gratte-ciel, 3: grand immeuble, 2: petit immeuble, 1: pavillon, 0: herbe

        # 0 à 40 -> gratte-ciel, 40 à 80 -> grand immeuble, 80 à 100 -> herbe
        if (distanceToCenter < 0.33f * radius){
            if (noise < 40)
                return 4;
            else if (noise >= 40 && noise < 80)
                return 3;
            else
                return 0;}
        # 0 à 40 -> grand immeuble, 40 à 80 -> petit immeuble, 80 à 100 -> herbe
        else if (distanceToCenter < 0.66f * radius){
            if (noise < 40)
                return 3;
            else if (noise >= 40 && noise < 80)
                return 2;
            else
                return 0;}
        # 0 à 40 -> petit immeuble, 40 à 70 -> pavillon, 70 à 100 -> herbe
        else if (distanceToCenter < radius){
            if (noise < 40)
                return 2;
            else if (noise >= 40 && noise < 70)
                return 1;
            else
                return 0;}
        # 0 à 50 -> pavillon, 50 à 100 -> herbe
        else if (distanceToCenter < 1.2f * radius){
            if (noise < 50)
                return 1;
            else
                return 0;}
        # 0 à 30 -> pavillon, 30 à 100 -> herbe
        else{
            if (noise < 30)
                return 1;
            else
                return 0;}
```

Voici le résultat obtenu avec cette nouvelle approche:

![noise+distance](https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/noise%2Bdistance.PNG)

  Cette méthode permet de créer des aires cohérentes avec un même asset. Elle est particulièrement efficace pour générer des espaces verts et des zones pavillonnaires en bordure de ville.

### Zoom dans le bruit

  Un autre avantage du bruit de Perlin est la possibilité de "zoomer" dans le bruit pour créer des zones unies plus grandes quand le zoom est plus important, et plus petites dans le cas inverse. Ce zoom ce traduit par l'implémentation de la variable _noiseSize_ dans le code.

```
int noise = (int)PerlinNoise(i / noiseSize + seed, j / noiseSize + seed) * 100);
```

  Grâce à cette variable, on peut obtenir des résultats très uniformes (grande valeur _noiseSize_) ou semblant presque aléatoires (très petite valeur de _noiseSize_).

<figure class="image">
  <img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/noise10.PNG" title="noiseSize = 10" alt="noise10" width="450"/>
  <figcaption> noiseSize = 10 </figcaption>
</figure>
<br>
<br>
<figure class="image">
  <img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/noise02.PNG" title="noiseSize = 2" alt="noise02" width="450"/>
  <figcaption> noiseSize = 2 </figcaption>
</figure>

## Conclusion

De toutes les méthodes étudiées, deux semblent donner de bons résulats: la méthode aléatoire+distance et la méthode bruit+distance.

<img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/random%2Bdistance2.PNG" title="random+distance" alt="random+distance" width="500"/> <img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/noise%2Bdistance.PNG" title="noise+distance" alt="noise+distance" width="500"/>

Cependant, ces deux méthodes sont efficaces dans des domaines différents. Là où la méthode aléatoire+distance réussit à créer des zones denses en gratte-ciels et immeubles grâce à la grande variance de taille des bâtiments, la méthode bruit+distance représente beaucoup mieux les espaces verts et les bordures de villes.

### Règles par quartier

Ainsi, la meilleure solution est certainement d'utiliser les deux méthodes, chacune en fonction du quartier que l'on veut créer. Pour générer un quartier pavillonnaire ou uniforme de par la taille des bâtiments, on utilisera une méthode à base de bruit de Perlin. Pour un quartier à l'architecture et aux hauteurs plus variables, on privilégiera une méthode employant une plus grosse part d'aléatoire.

Si on prend l'exemple de la ville de Lyon, pour un quartier comme Part-Dieu (photo de gauche), les tailles et l'architecture sont très variables d'un bâtiment à l'autre et une génération utilisant de l'aléatoire semble adapté. Pour le 1er arrondissement (photo de droite), une méthode générant des bâtiments de la même taille et de la même forme est ce cas bien meilleure.

<img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/PartDieu.jpg" title="Lyon Part-Dieu" alt="partDieu" width="400"/> <img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/1erLyon.jpg" align="right" title="Lyon 1er" alt="Lyon1er" width="430"/>

On peut donc imaginer une méthode qui établira, manuellement ou procéduralement, des règles de génération différentes pour chaque quartier.

### Travail de Niclas Olsson et Elias Frank

Comme travail allant dans le sens des règles par quartier, on peut citer celui de Niclas Olsson et Elias Frank. Dans leur article _Procedural city generation
using Perlin noise_, ils utilisent deux itérations de bruit de Perlin, une créant de grosses aires, la deuxième des aires plus petites. La première itération de bruit de Perlin permet de délimiter des quartiers, la seconde découpe ces quartiers en _blocs_ délimités par des routes.

<img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/districts.PNG" title="districts" alt="districts" width="400"/> <img src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/blocks.PNG" align="right" title="blocks" alt="blocks" width="400"/>

La particularité de leur implémentation est que chaque quartier possède ses propres paramètres de génération, dont les trois essentiels sont: la hauteur minimale, la hauteur maximale et la densité des bâtiments. Ainsi, cette méthode permet d'avoir, avec un même algorithme, à la fois des quartiers avec des bâtiments élevés aux tailles très variables et des quatiers avec uniquement des bâtiments d'un ou deux niveaux.

<p>
  <img align="left" alt="result" width="400" src="https://github.com/LorenzoMarnat/CityProceduralGeneration/blob/main/Screenshots/result.PNG">
  Sur cette image issue de l'article, on voit que les bâtiments du quartier en haut à gauche ont une importante variation de taille et une grande densité, alors que les autres quartiers sont plus uniformes et moins denses.  </p><br><br><br><br><br><br>
  
## Sources

_Procedural city generation using Perlin noise_, Niclas Olsson et Elias Frank : https://www.diva-portal.org/smash/get/diva2:1119094/FULLTEXT02

_Generating a Procedural City with Unity 5 Part 1_, Holistic3d  : https://youtu.be/xkuniXI3SEE

Photos de villes : https://jooinn.com/city-view-14.html

Texture d'herbe : https://assetstore.unity.com/packages/2d/textures-materials/floors/hand-painted-grass-texture-78552

Photo de Lyon 1er : https://www.sanofi.fr/fr/nous-connaitre/nos-sites-en-france/le-site-de-production-de-lyon-genzyme-polyclonals

Photo de Part-Dieu : https://fr.wikipedia.org/wiki/La_Part-Dieu
