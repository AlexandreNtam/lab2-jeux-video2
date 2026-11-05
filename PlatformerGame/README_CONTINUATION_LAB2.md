# Lab 2 : ce qu'il te reste à faire

D'après l'analyse de ton projet au commit `5e64ed7`.
Garde tes scripts actuels : pas besoin de recommencer le joueur.

## 1.1 - Gravité et scènes

Déjà configuré : gravité 2D à **-17** et cinq scènes dans le bon ordre.
Rien à refaire.

## 1.2 - Création du joueur

Ouvre `Assets/Prefabs/Player.prefab`, hors du mode Play.

- Sur le Rigidbody2D : mets **Collision Detection = Continuous**.
- Sur Feet : mets la **position locale Y à -0.32**. Garde la taille du
  BoxCollider2D à `(0.20, 0.04)` et Is Trigger coché.
- Dans **Project Settings > Physics 2D > Layer Collision Matrix** :
  pour PlayerFeet, garde seulement la collision avec Ground.
- Garde **Default / Ground** coché pour que le corps touche le sol.

La capsule, l'AudioSource et Freeze Rotation Z sont déjà présents.

## 1.3 - Déplacement

Déjà écrit dans `PlayerController`, avec une vitesse de **2**.

- Teste gauche/droite.
- Vérifie que LeftWall, RightWall et TopWall empêchent de sortir de l'écran.
- Ne refais pas les limites si elles fonctionnent.

## 1.4 - Contacts des pieds

Ton `PlayerGroundDetector` utilise déjà un compteur : garde-le.

- Vérifie que le saut fonctionne lorsque le joueur est posé.
- Vérifie qu'il reste au sol lorsqu'il passe entre deux blocs voisins.

## 1.5 - Saut de base

Le saut existe déjà. Dans `PlayerController`, fais passer le son par
SoundManager :

```csharp
audioSource.PlayOneShot(SoundManager.Instance.PlayerJump);
```

Le clip `jumpSound` directement assigné n'est alors plus nécessaire.
Vérifie Espace et le bouton sud de la manette.

## 1.6 - Gemmes

Ajoute un script `PlayerGems` sur le prefab Player.
Quand le joueur entre dans un trigger avec le tag **Gem** :

1. Désactive la gemme avec `SetActive(false)`.
2. Joue `SoundManager.Instance.GemCollected` sur l'AudioSource du joueur.
3. Appelle `GameManager.Instance.AddScore(50)`.

Ne détruis pas la gemme. Les gemmes déjà placées ont le bon tag.

## 1.7 - Score bonus

Ajoute un script `PlayerBonus` sur le prefab Player.

- Commence à **600** à chaque chargement de niveau.
- Retire **5 chaque seconde**, sans descendre sous zéro.
- Actualise l'affichage avec `GameManager.Instance.SetBonus(bonus)`.
- Arrête le décompte à la mort ou à la sortie.

Le texte du HUD existe déjà : ne crée pas de nouveau Canvas.

## 1.8 - Contact avec un monstre

Ajoute un script `PlayerState` sur le prefab Player.
Au contact du tag **Monster** :

1. Empêche le déplacement et les sauts; immobilise le joueur.
2. Appelle `GameManager.Instance.PlayerDie()` une seule fois.
3. Joue `SoundManager.Instance.PlayerKilled`.
4. Réduis le scale jusqu'à zéro pendant **1 seconde**.
5. Recharge le niveau après cette seconde.

Si tu attends déjà la seconde dans l'animation de mort, appelle ensuite
`GameManager.Instance.RestartLevel(0f)` pour ne pas attendre une seconde de plus.
Le GameManager choisit déjà GameOver lorsqu'il ne reste aucune vie.

## 1.9 - Sortie du joueur

Dans le même `PlayerState`, au contact du tag **Exit** :

1. Immobilise le joueur et arrête le décompte du bonus.
2. Ajoute le bonus restant avec `GameManager.Instance.AddScore(bonus)`.
3. Remplace la musique de la caméra par `SoundManager.Instance.ExitReached`,
   sans boucle, et joue-la.
4. Appelle `GameManager.Instance.StartNextlevel(2f)`.

Empêche le déclenchement de la mort ou de la sortie plusieurs fois.

## 1.10 - Ne plus rester collé aux murs

Corrige ta logique `isBlockedByWall` dans `PlayerController`.

- Dans Collision Enter et Stay : si le joueur est en l'air contre un
  obstacle statique, bloque sa vélocité X sans effacer Y.
- Efface le blocage quand il touche le sol.
- Efface aussi le blocage lorsqu'il quitte le mur.
- Vérifie qu'il continue à tomber lorsqu'il pousse contre un mur.

## 1.11 - Saut chargé

Tu l'as déjà commencé. Garde les valeurs :

```text
Vitesse initiale : 3
Vitesse maximale : 8
Temps de charge : 0.25 seconde
```

- Un appui bref doit donner un saut plus bas qu'un appui tenu.
- Relâcher doit terminer la charge.
- Réappuyer en l'air ne doit pas reprendre la charge.
- Arrête la charge si la montée est bloquée par un plafond.
- N'utilise pas de coroutine pour les contrôles.

## 1.12 - Coyote time

Ton compteur de **0.2 seconde** existe déjà, mais il faut le consommer :

- Remets le compteur à zéro dès que le joueur saute.
- Empêche un deuxième saut avant un véritable atterrissage.
- Réarme le saut après avoir quitté le sol puis atterri.
- Teste un saut juste après le bord d'une plateforme, puis après expiration.

## 1.13 - Prefab dans les trois niveaux

Le prefab Player existe déjà dans Scene1.

1. Ajoute les nouveaux scripts au **prefab**, pas seulement à son instance.
2. Place ce prefab dans **Scene2 et Scene3**.
3. Ajoute aussi les limites dans ces deux scènes.
4. Choisis un départ au-dessus du sol, sans contact avec un monstre.
5. Sauvegarde et teste les trois niveaux depuis Scene0.

Il faut un seul joueur par niveau.
Ne modifie pas Scene0, SceneGameOver ou les managers fournis.

## 1.14 - Bonus facultatif : murs

Améliore la tâche 1.10 : bloque X seulement quand le joueur pousse **vers**
le mur. Il doit pouvoir repartir dans l'autre direction même dans les airs.

Tu peux réduire temporairement la gravité pour tester.
Remets-la à **-17** ensuite.

## 1.15 - Bonus facultatif : animations

Anime le joueur avec les sprites Run, Jump, Die et Celebrate.
Fais ce bonus seulement après les tâches obligatoires.

## Remise

Teste le score, le bonus, les trois vies, les sorties et les trois niveaux.
Vérifie qu'il n'y a aucune erreur rouge dans la Console.

Le ZIP doit contenir seulement **Assets, Packages et ProjectSettings**,
avec leurs fichiers `.meta`. Un lien Git accessible est aussi permis.
