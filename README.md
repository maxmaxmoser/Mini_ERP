# Mini_ERP
Projet développé en C# par David Marck, Maxime Moser, Mohammed Zeammari, Sakina Mamodaly 

Lien vers le projet sur github : https://github.com/maxmaxmoser/Mini_ERP


Contexte : 
-----------
Le but du projet est de créer une application qui permet de calculer le nombre de ressources (chef de projet et développeur) nécessaire pour réaliser les projets saisis dans cette dernière. Elle permet de répondre aux questions de l'énoncé fourni par le professeur. 


Fichiers livrés : 
-----------------
- readMe.txt
- miniERP.exe (exécutable : affichage en mode console)
- les fichiers de paramétrage cas.json et projects.json
- simulationHumaine.xls (fichier excel dans lequel nous avons simulé manuellement les énoncés de chaque question. Le fichier explique la logique de calcul suivie pour répondre aux questions.)

Explication des fichiers de paramétrage : 
-----------------------------------------
Le fichier de paramétrage cas.json  prend en compte les paramètres suivants : 
- id (identifiant unique) : il est incrémental.
- date_depart (la date de départ ): la date de début prise en compte pour tous les projets . Date par défaut 01/06/2018.
- nb_dev (nombre de développeur) : Par défaut il y a 3 développeurs.
- nb_chef_proj (nombre de chef de projet ) : Par défaut il y a 1 chef de projet. 
- efficience (Efficience en % appliqué à chaque employé) : Par défaut l'efficience est à 100% pour tous les employés . Donc si on modifie l'efficience à 80% alors tous les employés auront une efficience à 80%.
- duree_embauche (la durée du processus d'embauche) : Par défaut elle dure 3 mois.
- duree_avt_efficacite (la durée au bout de laquelle les personnes embauchées sont efficaces) : Par défaut elle dure 1 mois.
- projects (le nom des projets à traiter) :  Par défaut ce sont les suivants : airbus,ninetendo,htc vr

Le fichier de paramétrage projects.json  prend en compte les paramètres suivants : 
- nom  : le nom du projet,
- nb_dev_days : le nombre de jours de développement,
- nb_mgt_days : le nombre de jours de gestion de projet,
- deadline : la date de livraison

----------------------------------------------------------------------------------------------------------------------------------------
Utilisation de la Console :
----------------------------
La console présente 3 options représentant les cas des questions A,B,C et D. Un booléen a été ajouté pour prendre en compte la question E. Donc pour toutes les questions si on a activé la question E, alors si un projet entraine un retard le processus d'embauche en fonction des ressources nécessaires est enclenchée, donc les nouveaux embauchés ne commenceront a travaillé que 4 mois (80 jrs) après leur embauche.


Pour choisir une option il suffit de saisir le numéro associé décrit dans la console. 


Pour rajouter un projet il suffit de rajouter une entrée dans le fichier projects.json.


Pour rajouter un cas (nombre d'employé, efficience ...) il suffit de rajouter une entrée dans le fichier cas.json.

Voir le Wiki créé sur le repository github https://github.com/maxmaxmoser/Mini_ERP/wiki pour une explication détaillée (avec images) de l'utilisation de l'exécutable.



----------------------------------------------------------------------------------------------------------------------------------------
Question A-B : 
--------------
Hypothèses :	
- Le premier projet commence le vendredi 1/6
- On réalise les projet en allant de celui à la deadline la plus proche à la plus éloignée
- Si des projets ont une deadline identique, on commence par le plus court
- Quand les employés finissent un projet, ils attaquent le suivant le jour de travail qui provient
- On résonne en semaine de travail (5 jours par semaine)
- Nous sommes constitués d'employés fidèles qui sont immunisé à toutes maladies, qui ne pose jamais congés et qui travaillent tous les jours férié
- Les projets ne sont pas réalisés en parallèle (un employé ne fait qu'un projet après l'autre)
- Les employés se répartissent les tâches de manière équitable
- Dès l'instant qu'un jour est entamé dans le temps de travail, on l'arrondi au supérieur (Ex : 18,25 jours : on indiquera qu'il faut 19 jours) 

Réponse : 				
Oui, nous somme en capacité de livrer les 3 solutions aux clients dans le délais imparti.				


Pour voir la simulation que nous avons effectuée, voir la page "Questions A-B" du fichier simulationHumaine.xls  



Question C :
------------
Hypothèses : 

- Toutes les hypothèses des questions suivantes sont appliquées
- Une efficience de travail à 80% implique qu'il faudra 20% de temps en plus pour réaliser un travail	

Réponse : 
Avec 80% d'efficcience, nous somme en capacité de livrer les 3 solutions aux clients dans le délais imparti.


Pour voir la simulation que nous avons effectuée, voir la page "Questions C" du fichier simulationHumaine.xls  


Question D : 
------------
Hypothèses :	
- Toutes les hypothèses des questions suivantes sont appliquées
- Une efficience de travail à 120% implique qu'il faudra 20% de temps en moins pour réaliser un travail	


Réponse : 															
Nous ne pouvons pas valider la date du 1/01/2019 pour livrer le projet SONI. D'après le tableau présenté dans le fichier simulationHumaine.xls, on remarque que 
la phase de développement est celle qui fait retarder le projet. Donc si on ajoute 2 ressources de développement seulements pour le projet de soni 
alors en ayant une efficacité de 120%  et en refaisant le calcul, le projet se termine le 31/12. 

Dans la théorie pure, la date serait résolvable. Néanmoins, celle-ci n'est pas vraiment raisonable. En effet  le projet serait terminé la veille, 
ce qui fait que le moindre retard ou incident par rapport à toute la planification pourrait empêcher le rendu avant la deadline.

La fin étant prévue le 31/12 en raison de la phase de gestion, nous préconisons d'y ajouter un nouveau gestionnaire (en contrat de sous-traitance) 
pendant au moins la moitiée du projet. La marge avant la deadline sera donc plus grande.


Pour voir la simulation que nous avons effectuée, voir la page "Questions D" du fichier simulationHumaine.xls  

Question E:
-----------
Hypothèses : 	
- Toutes les hypothèses des questions suivantes sont appliquées
- les processus d'embauche débutent le 01/06						
- On considère qu'un nouvel employé ne travaille pas sur un projet tant qu'il n'est pas effectif (4 mois, soit 80 jours)						

Pour voir la simulation que nous avons effectuée, voir la page "Questions E" du fichier simulationHumaine.xls  
