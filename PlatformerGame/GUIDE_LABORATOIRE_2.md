# Laboratoire 2 - Platformer : guide de réalisation

Ce guide est adapté au projet `PlatformerGame` et aux notes de cours fournies.
Il ne faut pas remplacer les scripts du professeur. Les scripts ci-dessous sont
à créer toi-même dans Unity : ce document ne les installe pas dans le jeu.

## 1. Ce qui est déjà fourni

Ouvre ce dossier dans Unity Hub :

```text
/home/david-alexandre/cegep/prog-jeu-video-2/lab2-jeux-video2/PlatformerGame
```

Utilise **Unity 6000.5.9f1**, conformément au cours 01.2 et à
`ProjectSettings/ProjectVersion.txt`. Ne change pas la version du projet.

| Élément | Situation dans le projet examiné |
|---|---|
| `Scene0` | Scène de titre terminée : ne pas la modifier |
| `SceneGameOver` | Scène de fin terminée : ne pas la modifier |
| `Scene1`, `Scene2`, `Scene3` | Niveaux construits, sans joueur |
| Input System | Installé et assigné aux actions globales du projet |
| `Move` | A/D, flèches et joystick déjà configurés |
| `Jump` | Espace et bouton sud déjà configurés |
| Canvas | Nom, score, vies et bonus déjà présents |
| `GameManager` | Score, vies, bonus et transitions déjà disponibles |
| `SoundManager` | Les quatre clips nécessaires sont déjà liés |
| `Platform` | `PlatformEffector2D` déjà configuré |
| Gravité 2D | À changer de `-9.81` à `-17` |
| Scene List | Seulement `Scene0` : ajouter les autres scènes |
| Joueur et limites | À créer |

Dans les niveaux, les `HardBlock` et `Platform` sont déjà sur le Layer physique
`Ground`, avec leurs tags respectifs.

**Travaille hors du mode Play lorsque tu crées les objets ou modifies leurs
paramètres. Sauvegarde chaque scène.** Les modifications faites pendant Play
ne sont normalement pas conservées à son arrêt.

## 2. Respect des notes et limites de ce guide

### 2.1 Concepts utilisés directement dans les cours

| Source locale | Application dans la solution |
|---|---|
| Cours 02.1 - Game objects, composants et MonoBehavior | `MonoBehaviour`, champs privés et `[SerializeField]` |
| Cours 02.2 - Prefabs et gestion des assets | Organisation, prefab, conservation des `.meta` |
| Cours 03.1 - Boucle de jeu et cycle de vie | `Start`, `Update`, `FixedUpdate`, `LateUpdate`, temps écoulé |
| Cours 03.2 - Pools et recyclage | `SetActive(false)` pour les gemmes |
| Cours 03.3 - Physique | Colliders, triggers, Layers et collisions |
| Cours 04.1, version du dossier Cours 05 | `FindAction`, `ReadValue`, `IsPressed`, `WasPressedThisFrame`, `WasReleasedThisFrame` |
| Cours 05.3 - Communications entre GameObjets | Références obtenues une fois avec `GetComponent` |
| Cours 06.3 - Intégration sonore | `AudioSource`, `PlayOneShot`, musique sur la caméra |
| Cours 16.1 - Unity 2D | Composants 2D, Sorting Layers, capsule, rotation gelée |
| Cours 16.2 - Platformers, p. 8-15 | Compteur aux pieds et correction des murs |
| Cours 16.2 - Platformers, p. 19 | Vitesse initiale et charge limitée dans le temps |
| Cours 16.2 - Platformers, p. 20-22 | Coyote time et suppression de la vitesse de chute au saut |
| Cours 16.3 - Gestion des scènes | Utilisation du manager persistant fourni |
| Cours 17.2 - Bonnes pratiques | Petits scripts, références fixes, managers non sérialisés |
| `Cours_03/Cours 03/Scripts/PlayerController.cs` | Utilisation de `linearVelocity` |
| `Assets/Scripts/RandomizeBlockSprite.cs` fourni | Tableau de sprites et changement de `SpriteRenderer.sprite` |
| Solutionnaire du cours 05, `TiresInContact.cs` | Compteur de contacts protégé contre les valeurs négatives |

Les algorithmes sont une proposition de réalisation, pas le solutionnaire du
professeur. Les notes décrivent notamment le saut chargé sans fournir son code
complet : le choix de calcul est expliqué à la section 7.

Pas de CharacterController 3D, de raycasts pour détecter le sol, de `AddForce`
pour sauter, de package supplémentaire, d'événements d'inputs complexes, ni de
coroutine pour lire ou gérer les contrôles.

### 2.2 API non détaillées dans les notes : sources explicites

Pour ne pas présenter une API supplémentaire comme si elle était enseignée,
voici les compléments utilisés. Les liens sont la documentation officielle
Unity 6, consultée pour ce guide.

| Complément | Pourquoi il est utilisé | Source officielle |
|---|---|---|
| Transmission des triggers de l'enfant au Rigidbody2D parent | L'enfant Feet peut ne porter que son BoxCollider2D; le script reste sur Player | [OnTriggerEnter2D](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/monobehaviour/ontriggerenter2d) |
| `Collision2D.collider`, `rigidbody`, `contactCount` | Reconnaître l'obstacle statique et examiner ses contacts | [Collision2D](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/collision2d) |
| `Collision2D.GetContact(i)` | Lire un contact sans créer un tableau de contacts à chaque callback | [GetContact](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/collision2d/getcontact) |
| `ContactPoint2D.normal` | Distinguer le côté d'un mur du dessus d'un bloc | [normal](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/contactpoint2d/normal) |
| `Rigidbody2D.simulated` dans le code | Immobiliser totalement le joueur à la mort ou à la sortie | [simulated](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/rigidbody2d/simulated) |
| `Camera.main` | Retrouver une fois la caméra et son AudioSource musical | [Camera.main](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/main) |
| Découpage par grille dans Sprite Editor, seulement pour le bonus 2 | Uniformiser les cadres et pivots des frames | [Cut out sprites from a texture](https://docs.unity.com/en-us/engine/6000.0/manual/unity2d/sprite/use-editor) |

Pour les tags, on utilise simplement `other.gameObject.tag == "Gem"`, dans le
même esprit que les tests de tags déjà présents dans le code fourni.
Pas besoin d'apprendre `CompareTag`.

Les calculs utilisent seulement des opérations arithmétiques et des `if`.
Pas de `Mathf.Lerp`, de collections dynamiques ou de système Animator à apprendre.

## 3. Préparer les scènes, la gravité et les inputs

### 3.1 Liste des scènes - tâche 1.1

1. Ouvre **File > Build Profiles > Scene List**.
2. Ajoute les scènes de `Assets/Scenes`.
3. Coche les cinq scènes et mets-les dans cet ordre :

| Index | Scène |
|---|---|
| 0 | `Scene0` |
| 1 | `Scene1` |
| 2 | `Scene2` |
| 3 | `Scene3` |
| 4 | `SceneGameOver` |

Le manager utilise l'index de la scène lorsqu'on commence directement dans un
niveau pour développer. Il faut donc bien respecter les index 1, 2 et 3.

### 3.2 Gravité

Dans **Edit > Project Settings > Physics 2D** :

```text
Gravity X : 0
Gravity Y : -17
Simulation Mode : Fixed Update
```

Ne change pas la gravité de Physics 3D ni le Fixed Timestep.

### 3.3 Inputs

Sélectionne `Assets/Inputs/InputSystem_Actions.inputactions`.
Vérifie simplement que `Move` et `Jump` sont présents dans la map Player.
Leurs bindings sont déjà corrects dans ce projet.

Les scripts utiliseront la méthode du cours :

```text
InputSystem.actions.FindAction("Move")
InputSystem.actions.FindAction("Jump")
```

Ne rajoute ni composant PlayerInput, ni classe générée, ni appel à `.Enable()`.
Si Unity affiche « Assign as the Project-wide Input Actions », assure-toi que
cet asset est assigné. Il l'est déjà dans la configuration examinée.

## 4. Construire Player dans Scene1 - tâche 1.2

### 4.1 Sprite et position

1. Ouvre `Assets/Scenes/Scene1`.
2. Glisse `Assets/Sprites/Player/Idle.png` dans la Hierarchy.
3. Renomme l'objet `Player`.
4. Mets le Transform :

```text
Position : (-3.4, -1.75, 0)
Rotation : (0, 0, 0)
Scale    : (1, 1, 1)
```

Cette position de départ est à gauche au-dessus du plancher. Ajuste-la au besoin
après le premier test, sans placer le joueur à l'intérieur d'un bloc.

Dans le Sprite Renderer :

```text
Sorting Layer : Player
Order in Layer : 0
```

Le Sorting Layer `Player` existe déjà. Garde le **Layer physique** du GameObject
à `Default`. Le Sorting Layer règle l'affichage; le Layer physique intervient
notamment dans les collisions.

### 4.2 Rigidbody2D

Ajoute **Rigidbody 2D**, puis configure :

| Paramètre | Valeur |
|---|---|
| Body Type | Dynamic |
| Simulated | Coché |
| Gravity Scale | 1 |
| Linear Damping | 0 |
| Collision Detection | Continuous |
| Constraints > Freeze Rotation Z | Coché |
| Freeze Position X et Y | Décochés |

Dynamic permet de subir la gravité et les collisions. Continuous améliore leur
détection. Freeze Rotation Z empêche le personnage de tourner contre les blocs.

### 4.3 CapsuleCollider2D

Ajoute **Capsule Collider 2D** :

```text
Is Trigger : décoché
Direction : Vertical
Size : (0.26, 0.56)
Offset : (0, -0.02)
```

Ces dimensions sont des valeurs de départ, pas des mesures imposées par
l'énoncé. Avec **Edit Collider**, entoure le corps visible plutôt que les pixels
transparents. Le bas de cette capsule est à environ `-0.30` en coordonnées locales.

Ne coche pas Is Trigger : cette capsule doit arrêter physiquement le joueur.
N'ajoute pas de matériau sans friction pour remplacer le travail demandé sur
les murs.

### 4.4 Enfant Feet

1. Clic droit sur Player > **Create Empty**.
2. Renomme l'enfant `Feet`.
3. Mets son Transform local :

```text
Position : (0, -0.30, 0)
Rotation : (0, 0, 0)
Scale    : (1, 1, 1)
```

4. Ajoute **Box Collider 2D** :

```text
Is Trigger : coché
Size : (0.20, 0.04)
Offset : (0, 0)
```

La boîte va de `-0.28` à `-0.32`, donc dépasse légèrement la capsule vers le bas.
Si tu ajustes la capsule, ajuste aussi la position des pieds.
Garde la boîte étroite pour ne pas considérer facilement un mur latéral comme
du sol.

**Feet porte seulement Transform et BoxCollider2D. Pas de script, pas de
Rigidbody2D, pas d'AudioSource.** Le Rigidbody2D du parent prend en charge
l'ensemble; voir la source officielle en section 2.2 pour les callbacks.

### 4.5 Layer PlayerFeet et matrice

1. Sélectionne Feet > **Layer > Add Layer**.
2. Crée `PlayerFeet` dans un emplacement utilisateur libre.
3. Attribue ce Layer à Feet seulement.
4. Dans **Project Settings > Physics 2D > Layer Collision Matrix**, garde
   seulement `PlayerFeet <-> Ground` coché pour les interactions de PlayerFeet.

| Paire | État |
|---|---|
| PlayerFeet / Ground | Cochée |
| PlayerFeet / Default | Décochée |
| PlayerFeet / PlayerFeet | Décochée |
| PlayerFeet / tous les autres Layers | Décochée |
| Default / Ground | Reste cochée |

Ne change pas inutilement le reste de la matrice. La capsule du parent sur
Default doit continuer à collisionner avec Ground et les triggers sur Default.

### 4.6 AudioSource

Sur **Player**, ajoute un seul **Audio Source** :

```text
Audio Resource : None
Play On Awake : décoché
Loop : décoché
Spatial Blend : 0, son 2D
Volume : 1
```

Les clips viendront du SoundManager. Les caméras des niveaux ont déjà leur
AudioSource pour la musique et leur AudioListener.

## 5. Limites invisibles - tâche 1.3

Les planchers actuels vont approximativement de X = -4 à X = 4.
Ajoute des colliders plutôt que des limites de position codées.

1. Crée un GameObject vide `LevelLimits` à position et rotation zéro, scale un.
2. Crée ces trois enfants, chacun avec seulement un Transform et un
   BoxCollider2D non trigger :

| Enfant | Position locale | Size du BoxCollider2D |
|---|---|---|
| LeftWall | (-4.1, 0, 0) | (0.2, 12) |
| RightWall | (4.1, 0, 0) | (0.2, 12) |
| TopWall | (0, 2.91, 0) | (10.4, 0.2) |

Pour chacun : Layer `Default`, Tag `Untagged`, Offset `(0, 0)`.
Pas de sprite, pas de Rigidbody2D. Ces colliders sont statiques.

Les côtés empêchent de quitter le plancher; le plafond empêche de sortir par le
haut. Le plancher existant empêche la sortie par le bas.
La caméra des niveaux a une taille orthographique de `2.8075`.

Choisis **16:9** dans la fenêtre Game, cohérent avec la résolution actuelle
1920 x 1080. Ces positions supposent ce cadrage. Si tu changes le ratio ou la
caméra, revérifie les limites : ce guide ne fait pas de redimensionnement
automatique des murs.

## 6. Scripts et compteur de sol - tâche 1.4

Dans `Assets/Scripts`, crée les cinq fichiers suivants. Le nom de chaque fichier
doit être exactement le nom de sa classe. Attache les cinq à **Player**.

| Fichier | Rôle |
|---|---|
| PlayerGroundDetector.cs | Compter les contacts des pieds |
| PlayerMovement.cs | Construire le mouvement |
| PlayerBonus.cs | Décompter le bonus |
| PlayerGems.cs | Ramasser les gemmes |
| PlayerState.cs | Gérer mort et sortie |

Crée tous les fichiers avant de tester : certaines classes se référencent.
Ne remplace pas GameManager.cs, SoundManager.cs ou les autres scripts fournis.

### PlayerGroundDetector.cs

```csharp
using UnityEngine;

public class PlayerGroundDetector : MonoBehaviour
{
    private int groundContacts = 0;

    public bool IsGrounded()
    {
        return groundContacts > 0;
    }

    private bool IsGround(Collider2D other)
    {
        return other.gameObject.tag == "HardBlock"
            || other.gameObject.tag == "Platform";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsGround(other))
        {
            groundContacts++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsGround(other))
        {
            groundContacts--;

            if (groundContacts < 0)
            {
                groundContacts = 0;
            }
        }
    }
}
```

Deux blocs touchés donnent deux contacts. En quittant un seul bloc, il reste un
contact : le joueur reste au sol. À zéro, il n'est plus au sol.
C'est le compteur exigé, pas un simple booléen mémorisé.

La matrice empêche Feet de toucher les gemmes ou monstres; les tests de tags
séparent ensuite les contacts de sol des autres triggers reçus par Player.

## 7. Mouvement, saut, charge, coyote time et murs

### PlayerMovement.cs

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float initialJumpSpeed = 3f;
    [SerializeField] private float maximumJumpSpeed = 8f;
    [SerializeField] private float chargeDuration = 0.25f;
    [SerializeField] private float coyoteDuration = 0.2f;

    private Rigidbody2D rigidBody;
    private PlayerGroundDetector groundDetector;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveValue;
    private bool jumpRequested;
    private bool jumpHeld;

    private bool canMove = true;
    private bool jumpUsed;
    private bool leftGroundAfterJump;
    private bool chargingJump;
    private float chargeTime;
    private float coyoteTime;

    private Collider2D wallCollider;
    private int wallDirection;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<PlayerGroundDetector>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        moveValue = moveAction.ReadValue<Vector2>();
        jumpHeld = jumpAction.IsPressed();

        if (jumpAction.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

        if (jumpAction.WasReleasedThisFrame())
        {
            chargingJump = false;
        }

        if (moveValue.x != 0f)
        {
            spriteRenderer.flipX = moveValue.x < 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        UpdateCoyoteTime();

        if (jumpRequested
            && !jumpUsed
            && (groundDetector.IsGrounded() || coyoteTime > 0f))
        {
            StartJump();
        }
        else
        {
            UpdateJumpCharge();
        }

        jumpRequested = false;

        float horizontalSpeed = moveValue.x * moveSpeed;

        if (!groundDetector.IsGrounded()
            && wallDirection != 0
            && moveValue.x * wallDirection > 0f)
        {
            horizontalSpeed = 0f;
        }

        rigidBody.linearVelocity = new Vector2(
            horizontalSpeed,
            rigidBody.linearVelocity.y
        );
    }

    private void UpdateCoyoteTime()
    {
        bool grounded = groundDetector.IsGrounded();

        if (!grounded)
        {
            leftGroundAfterJump = true;
        }

        if (grounded)
        {
            ClearWall();
        }

        if (grounded
            && rigidBody.linearVelocity.y <= 0f
            && (!jumpUsed || leftGroundAfterJump))
        {
            coyoteTime = coyoteDuration;
            jumpUsed = false;
        }
        else
        {
            coyoteTime -= Time.fixedDeltaTime;

            if (coyoteTime < 0f)
            {
                coyoteTime = 0f;
            }
        }
    }

    private void StartJump()
    {
        jumpUsed = true;
        leftGroundAfterJump = false;
        coyoteTime = 0f;
        chargeTime = 0f;
        chargingJump = jumpHeld && chargeDuration > 0f;
        ClearWall();

        rigidBody.linearVelocity = new Vector2(
            rigidBody.linearVelocity.x,
            initialJumpSpeed
        );

        audioSource.PlayOneShot(SoundManager.Instance.PlayerJump);
    }

    private void UpdateJumpCharge()
    {
        if (!chargingJump)
        {
            return;
        }

        if (!jumpHeld || rigidBody.linearVelocity.y <= 0f)
        {
            chargingJump = false;
            return;
        }

        chargeTime += Time.fixedDeltaTime;

        if (chargeTime >= chargeDuration)
        {
            chargeTime = chargeDuration;
            chargingJump = false;
        }

        float progress = chargeTime / chargeDuration;
        float verticalSpeed = initialJumpSpeed
            + (maximumJumpSpeed - initialJumpSpeed) * progress;

        rigidBody.linearVelocity = new Vector2(
            rigidBody.linearVelocity.x,
            verticalSpeed
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckWall(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckWall(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == wallCollider)
        {
            ClearWall();
        }
    }

    private void CheckWall(Collision2D collision)
    {
        if (!canMove || groundDetector.IsGrounded())
        {
            ClearWall();
            return;
        }

        if (collision.rigidbody != null
            && collision.rigidbody.bodyType != RigidbodyType2D.Static)
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;

            if (normal.x < -0.5f || normal.x > 0.5f)
            {
                wallCollider = collision.collider;

                if (normal.x > 0f)
                {
                    wallDirection = -1;
                }
                else
                {
                    wallDirection = 1;
                }

                if (moveValue.x * wallDirection > 0f)
                {
                    rigidBody.linearVelocity = new Vector2(
                        0f,
                        rigidBody.linearVelocity.y
                    );
                }

                return;
            }
        }

        if (collision.collider == wallCollider)
        {
            ClearWall();
        }
    }

    private void ClearWall()
    {
        wallCollider = null;
        wallDirection = 0;
    }

    public void StopMoving()
    {
        canMove = false;
        jumpRequested = false;
        chargingJump = false;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.simulated = false;
        ClearWall();
    }
}
```

### 7.1 Pourquoi ce découpage?

`Update` lit les inputs. `FixedUpdate` construit la vélocité du Rigidbody2D.
`jumpRequested` est simplement un booléen qui garde un appui jusqu'au prochain
cycle physique : ce n'est pas un jump buffer de gameplay ni un système d'events.
La demande est consommée au prochain FixedUpdate, même si elle est refusée.

La vélocité X est `moveValue.x * 2`. La vélocité Y est conservée lorsqu'on ne
contrôle pas la charge. On ne multiplie pas une vélocité assignée par deltaTime :
la physique s'occupe déjà de la convertir en déplacement.

### 7.2 Tester le saut de base - tâche 1.5

Dans l'Inspector, mets temporairement :

```text
Initial Jump Speed : 8
Maximum Jump Speed : 8
Charge Duration : 0
Coyote Duration : 0
```

Teste le saut au sol et le son. Ces réglages désactivent charge et coyote time.
Les champs sérialisés dans l'Inspector priment sur les valeurs écrites dans le
code : vérifie toujours l'Inspector.

### 7.3 Activer la charge - tâche 1.11

Réglages finaux :

```text
Initial Jump Speed : 3
Maximum Jump Speed : 8
Charge Duration : 0.25
```

On donne immédiatement 3 au saut. Tant que le bouton reste tenu, pendant au
maximum 0.25 seconde, on impose :

```text
vitesse Y = 3 + (8 - 3) * (temps de charge / 0.25)
```

Le supplément de vitesse imposée passe donc progressivement de 0 à 5.
C'est un calcul de proportion avec les opérations de base, pas une nouvelle
API mathématique.

**Choix d'implémentation explicite :** les notes demandent de distribuer la
vélocité selon le temps, sans préciser comment composer cela avec la gravité.
Ici, on réassigne la vitesse Y visée durant la charge pour atteindre la valeur
imposée de 8. La physique applique toujours sa gravité après les écritures de
vélocité; après la charge, on ne réassigne plus Y.
La vitesse mesurée après un pas physique peut donc être légèrement inférieure
à la vitesse que le script vient d'imposer.

Une autre interprétation serait d'ajouter seulement `20 * fixedDeltaTime` à
la vitesse physique actuelle. Mais avec la gravité -17, on n'obtiendrait pas
réellement une montée de la vitesse de 3 à 8 en 0.25 seconde. Ne mélange pas ces
deux méthodes dans le même script.

Relâcher le bouton termine la charge, même si on le réappuie ensuite en plein
saut. Toucher un plafond et perdre sa vitesse ascendante la termine également.
Tenir indéfiniment ne prolonge pas la charge.
Garde les vitesses positives, la durée de charge positive ou zéro, et la vitesse
maximale supérieure ou égale à la vitesse initiale.

### 7.4 Coyote time - tâche 1.12

Mets `Coyote Duration = 0.2`.

Au sol, le délai est disponible. En quittant le sol sans sauter, il descend à
zéro. Le joueur peut sauter tant qu'il reste du temps. Au saut, le délai est
consommé et `jumpUsed` empêche un deuxième saut.

`leftGroundAfterJump` évite de réarmer un saut avant que les pieds aient vraiment
quitté le sol : utile si la boîte chevauche encore le plancher ou si le joueur
frappe immédiatement un plafond bas.
Un atterrissage descendant réarme le saut.

La vitesse de chute est remplacée par la vitesse initiale : le saut en coyote
time n'est pas affaibli par une vitesse verticale négative.
Un joueur qui commence dans les airs n'a pas de coyote time disponible.

### 7.5 Murs - tâche 1.10 et bonus 1

La correction simple des notes supprime X contre un obstacle statique dans les
airs. Cette version ajoute directement la vérification directionnelle du
**bonus 1** :

```text
mur à droite + input droite : X = 0
mur à droite + input gauche : déplacement permis
mur à gauche + input gauche : X = 0
mur à gauche + input droite : déplacement permis
```

Une normale est un vecteur perpendiculaire à la surface touchée.
Une forte composante X correspond au côté d'un obstacle :
normale positive en X = obstacle à gauche; normale négative = obstacle à droite.
Les API de contact sont un complément sourcé en section 2.2, pas du code
prétendument copié des notes.

On vérifie Enter et Stay. On efface l'état sur Exit, au sol, au saut et à l'arrêt.
On garde Y pour que le joueur continue à tomber.
La variable `wallCollider` mémorise l'obstacle latéral courant : solution simple
adaptée aux niveaux fournis, pas un contrôleur général pour un labyrinthe
arbitraire avec plusieurs murs simultanés.

## 8. Bonus de score - tâche 1.7

### PlayerBonus.cs

```csharp
using UnityEngine;

public class PlayerBonus : MonoBehaviour
{
    [SerializeField] private int startingBonus = 600;
    [SerializeField] private int lossPerSecond = 5;

    private int currentBonus;
    private float elapsedTime;
    private bool counting = true;

    private void Start()
    {
        currentBonus = startingBonus;
        GameManager.Instance.SetBonus(currentBonus);
    }

    private void Update()
    {
        if (!counting || currentBonus <= 0)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        while (elapsedTime >= 1f && currentBonus > 0)
        {
            elapsedTime -= 1f;
            currentBonus -= lossPerSecond;

            if (currentBonus < 0)
            {
                currentBonus = 0;
            }

            GameManager.Instance.SetBonus(currentBonus);
        }
    }

    public int GetBonus()
    {
        return currentBonus;
    }

    public void StopCountdown()
    {
        counting = false;
    }
}
```

Le bonus fait 600, 595, 590, etc. Il ne devient pas négatif.
Le `while` rattrape plusieurs secondes si un frame est exceptionnellement long.
Garde les deux paramètres positifs.

Chaque nouveau Player commence un nouveau bonus : après une transition ou une
mort, il revient à 600.
`SetBonus` existe déjà et actualise le texte du Canvas.
**Ne crée pas un nouveau HUD et ne remplace pas les Text Legacy par du TMP** :
le GameManager fourni utilise `UnityEngine.UI.Text`.

## 9. Gemmes - tâche 1.6

### PlayerGems.cs

```csharp
using UnityEngine;

public class PlayerGems : MonoBehaviour
{
    [SerializeField] private int pointsPerGem = 50;

    private AudioSource audioSource;
    private PlayerState playerState;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerState = GetComponent<PlayerState>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerState.HasFinished())
        {
            return;
        }

        if (other.gameObject.tag == "Gem"
            && other.gameObject.activeSelf)
        {
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(SoundManager.Instance.GemCollected);
            GameManager.Instance.AddScore(pointsPerGem);
        }
    }
}
```

La gemme est désactivée, le joueur joue le son et le manager ajoute 50 points.
Le son reste sur Player : désactiver la gemme ne le coupe pas.
Pas de Destroy.

Dans le projet examiné, toutes les gemmes placées dans les trois niveaux ont le
tag `Gem`. Le prefab Gem lui-même est cependant Untagged.
Si tu ajoutes des gemmes depuis le prefab, donne-leur le tag Gem ou attribue ce
tag au prefab pour les nouvelles instances. Ne retouche pas les gemmes
existantes inutilement.

## 10. Mort et sortie - tâches 1.8 et 1.9

### PlayerState.cs

```csharp
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerBonus bonus;
    private AudioSource audioSource;
    private AudioSource musicSource;

    private bool finished;
    private bool dead;
    private bool reloadRequested;
    private float deathTime;
    private Vector3 deathStartScale;

    private const float DEATH_DURATION = 1f;
    private const float EXIT_DELAY = 2f;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        bonus = GetComponent<PlayerBonus>();
        audioSource = GetComponent<AudioSource>();
        musicSource = Camera.main.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!dead || reloadRequested)
        {
            return;
        }

        deathTime += Time.deltaTime;
        float progress = deathTime / DEATH_DURATION;

        if (progress > 1f)
        {
            progress = 1f;
        }

        transform.localScale = deathStartScale * (1f - progress);

        if (deathTime >= DEATH_DURATION)
        {
            transform.localScale = Vector3.zero;
            reloadRequested = true;
            GameManager.Instance.RestartLevel(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finished)
        {
            return;
        }

        if (other.gameObject.tag == "Monster")
        {
            Die();
        }
        else if (other.gameObject.tag == "Exit")
        {
            ReachExit();
        }
    }

    private void Die()
    {
        finished = true;
        dead = true;
        deathTime = 0f;
        deathStartScale = transform.localScale;

        movement.StopMoving();
        bonus.StopCountdown();
        musicSource.Stop();
        audioSource.PlayOneShot(SoundManager.Instance.PlayerKilled);
        GameManager.Instance.PlayerDie();
    }

    private void ReachExit()
    {
        finished = true;
        movement.StopMoving();
        bonus.StopCountdown();
        GameManager.Instance.AddScore(bonus.GetBonus());

        musicSource.clip = SoundManager.Instance.ExitReached;
        musicSource.loop = false;
        musicSource.Play();

        GameManager.Instance.StartNextlevel(EXIT_DELAY);
    }

    public bool HasFinished()
    {
        return finished;
    }

    public bool IsDead()
    {
        return dead;
    }

    public bool HasWon()
    {
        return finished && !dead;
    }
}
```

### 10.1 Mort

Un contact Monster :

1. Bloque les contrôles et la simulation du joueur.
2. Arrête le bonus et la musique normale.
3. Joue PlayerKilled.
4. Retire une vie avec `GameManager.Instance.PlayerDie()`.
5. Réduit le scale de sa valeur initiale à zéro durant une seconde.
6. Demande le rechargement une seule fois.

`RestartLevel(0f)` est intentionnel : la seconde d'attente a déjà été passée à
rétrécir. Faire attendre une autre seconde au manager donnerait deux secondes
au total. La transition effective peut demander un frame supplémentaire, car
le manager utilise une coroutine.
On garantit ainsi le scale zéro avant de demander la transition.

Le GameManager choisit déjà SceneGameOver si les vies sont à zéro. Ne duplique
pas cette règle dans PlayerState.
`finished` empêche plusieurs pertes de vie ou une sortie après la mort.
`reloadRequested` empêche plusieurs demandes de rechargement.

### 10.2 Sortie

Un contact Exit :

1. Arrête le joueur et le décompte.
2. Ajoute le bonus restant au score une seule fois.
3. Remplace la musique de la caméra par ExitReached, sans boucle.
4. Demande le prochain niveau avec deux secondes de délai.

Les caméras des trois niveaux ont déjà le tag MainCamera et un AudioSource.
Camera.main et simulated sont les compléments sourcés de la section 2.2.
Arrêter la simulation ne désactive pas Player : son son et son Update restent
actifs pendant le rétrécissement.

Après Scene3, le manager fourni revient à Scene1. GameOver est la conséquence
de zéro vie, pas de la fin du troisième niveau. Garde ce fonctionnement fourni.

## 11. Configuration complète et tests progressifs

La hiérarchie doit être :

```text
Player
    Transform
    Sprite Renderer
    Rigidbody2D
    CapsuleCollider2D
    AudioSource
    PlayerGroundDetector
    PlayerMovement
    PlayerBonus
    PlayerGems
    PlayerState
    Feet
        Transform
        BoxCollider2D
```

Les cinq scripts n'ont aucune référence à glisser : leurs GetComponent sont
faits une seule fois au Start. Les références aux managers ne sont pas des
champs sérialisés, conformément aux bonnes pratiques du cours.

### Ordre de travail conseillé

1. Liste des scènes, gravité, construction du joueur et limites.
2. Compteur aux pieds et mouvement horizontal.
3. Saut de base : réglages 8 / 8 / 0, coyote à zéro.
4. Gemmes, bonus, mort et sortie.
5. Saut chargé : réglages 3 / 8 / 0.25.
6. Coyote time : 0.2.
7. Test des murs et du bonus 1.
8. Prefab dans les trois niveaux.
9. Bonus d'animation seulement après le fonctionnement obligatoire.

Tous les scripts peuvent être créés ensemble; cet ordre décrit les
fonctionnalités à tester progressivement.

### Tests à faire dans Unity

| Test | Résultat attendu |
|---|---|
| Chute sur le plancher | La capsule arrête le joueur, sans rotation |
| Mouvement | Gauche et droite, sans sortie de l'écran |
| Joints du plancher | Pas de blocage entre deux tuiles |
| Saut de base | Vitesse initiale 8, son, pas de double saut |
| Deux blocs sous les pieds | Quitter un seul bloc ne retire pas l'état au sol |
| Gemme | Désactivée, son, +50 une seule fois |
| Bonus | 600 au départ, -5 par seconde, minimum zéro |
| Monstre | Une vie perdue, aucun mouvement, scale zéro en une seconde |
| Dernière vie | SceneGameOver après l'animation de mort |
| Sortie | Bonus ajouté, musique de victoire, prochain niveau après deux secondes |
| Charge | Appui bref plus bas qu'appui tenu, charge limitée |
| Relâchement/réappui en l'air | Ne reprend pas la charge et ne donne pas un nouveau saut |
| Plafond | Ne reste pas poussé contre le plafond par la charge |
| Coyote | Saut permis juste après le bord, refusé après expiration |
| Spawn dans les airs | Pas de saut gratuit avant le premier contact au sol |
| Mur en l'air | Continue à tomber; peut repartir dans l'autre direction |

Pour observer le bonus 1, diminue temporairement la gravité comme le demande
l'énoncé. **Remets-la à -17 ensuite.**
Pour observer le coyote time, utilise une plateforme surélevée : le plancher
de base n'a pas de trou dans les niveaux fournis.

## 12. Prefab et autres niveaux - tâche 1.13

1. Arrête Play et sauvegarde Scene1.
2. Glisse Player complet, avec Feet, de la Hierarchy vers `Assets/Prefabs`.
3. Nomme le prefab `Player`.
4. Fais aussi un prefab de LevelLimits pour réutiliser ses colliders.
5. Ouvre Scene2, glisse les deux prefabs et sauvegarde.
6. Fais pareil dans Scene3.

Un point de départ possible dans les trois niveaux :

```text
Player Position : (-3.4, -1.75, 0)
```

Vérifie visuellement que la capsule n'est pas dans un bloc et que le personnage
n'apparaît pas au contact du monstre. Ajuste chaque départ au besoin.
Il doit y avoir un Player par niveau et aucun dans les scènes de titre/GameOver.

Réglages finaux du prefab :

| Champ | Valeur |
|---|---|
| Move Speed | 2 |
| Initial Jump Speed | 3 |
| Maximum Jump Speed | 8 |
| Charge Duration | 0.25 |
| Coyote Duration | 0.2 |
| Starting Bonus | 600 |
| Loss Per Second | 5 |
| Points Per Gem | 50 |

Modifie les paramètres communs dans le prefab, pas seulement sur une instance.
Sinon utilise Overrides > Apply pour les modifications communes voulues.
Garde la position de départ propre à chaque scène.

Teste enfin depuis Scene0, en entrant un nom d'au moins trois caractères :

```text
Scene0 -> Scene1 -> Scene2 -> Scene3 -> Scene1
```

Score et vies se conservent. Bonus revient à 600.
Teste aussi trois morts et le retour depuis GameOver.
Ne modifie pas les scripts de ces scènes pour réaliser les tâches du joueur.

## 13. Bonus 2 : animation simple et facultative

L'énoncé laisse l'animation facultative et les notes ne détaillent pas un
contrôleur Animator complet. Pour ne pas ajouter ce système, on peut animer
par tableaux de sprites, comme le projet change déjà ses sprites de blocs,
avec un compteur de temps dans LateUpdate.

Cela satisfait l'idée d'animer le personnage sans ajouter de notion de
transitions Animator. Le découpage précis dans Sprite Editor est un complément
officiel sourcé en section 2.2.

### 13.1 Préparer les feuilles

Dans `Assets/Sprites/Player`, utilise Run, Jump, Die et Celebrate.
Idle reste le sprite de départ.
Les cadres/pivots existants ne sont pas tous uniformes.

Pour chacune des quatre feuilles :

1. Sélectionne la texture dans Project.
2. Vérifie Texture Type = Sprite (2D and UI), Sprite Mode = Multiple,
   Pixels Per Unit = 100.
3. Ouvre Sprite Editor > Slice.
4. Choisis Grid By Cell Size, 64 x 64, Pivot Center.
5. Remplace les anciens rectangles plutôt que d'ajouter des doublons
   (méthode Delete Existing si elle est proposée).
6. Slice puis Apply.

Ces feuilles utilisent des cellules 64 x 64. Uniformiser les cellules et pivots
évite que le corps se décale en changeant de frame.
Ne fais pas ce travail si tu ne réalises pas le bonus 2.
Il ne faut pas animer la taille ou la position des colliders.

### PlayerAnimation.cs

```csharp
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] runFrames;
    [SerializeField] private Sprite[] jumpFrames;
    [SerializeField] private Sprite[] deathFrames;
    [SerializeField] private Sprite[] victoryFrames;
    [SerializeField] private float framesPerSecond = 12f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    private PlayerGroundDetector groundDetector;
    private PlayerState playerState;
    private Sprite idleSprite;
    private Sprite[] currentFrames;
    private int frameIndex;
    private float elapsedTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<PlayerGroundDetector>();
        playerState = GetComponent<PlayerState>();
        idleSprite = spriteRenderer.sprite;
    }

    private void LateUpdate()
    {
        Sprite[] nextFrames = null;

        if (playerState.IsDead())
        {
            nextFrames = deathFrames;
        }
        else if (playerState.HasWon())
        {
            nextFrames = victoryFrames;
        }
        else if (!groundDetector.IsGrounded()
            || rigidBody.linearVelocity.y > 0.1f)
        {
            nextFrames = jumpFrames;
        }
        else if (rigidBody.linearVelocity.x < -0.01f
            || rigidBody.linearVelocity.x > 0.01f)
        {
            nextFrames = runFrames;
        }

        if (nextFrames == null || nextFrames.Length == 0)
        {
            currentFrames = null;
            frameIndex = 0;
            elapsedTime = 0f;
            spriteRenderer.sprite = idleSprite;
            return;
        }

        if (nextFrames != currentFrames)
        {
            currentFrames = nextFrames;
            frameIndex = 0;
            elapsedTime = 0f;
        }

        float speed = framesPerSecond;

        if (speed < 1f)
        {
            speed = 1f;
        }

        float frameDuration = 1f / speed;
        elapsedTime += Time.deltaTime;

        while (elapsedTime >= frameDuration)
        {
            elapsedTime -= frameDuration;
            frameIndex++;

            if (frameIndex >= currentFrames.Length)
            {
                frameIndex = 0;
            }
        }

        spriteRenderer.sprite = currentFrames[frameIndex];
    }
}
```

Ajoute ce script au prefab Player. Développe chaque feuille dans Project pour
voir ses sous-sprites. Remplis les tableaux dans l'ordre numérique des frames :

| Tableau | Sous-sprites à mettre |
|---|---|
| Run Frames | Run_0, Run_1, etc. |
| Jump Frames | Jump_0, Jump_1, etc. |
| Death Frames | Die_0, Die_1, etc. |
| Victory Frames | Celebrate_0, Celebrate_1, etc. |

Vérifie l'ordre toi-même : un classement alphabétique peut placer `_10` avant
`_2`. Mets Frames Per Second à 12 et Idle comme sprite initial.
N'ajoute pas d'Animator en parallèle avec ce script.

Immobile = Idle; déplacement = Run; air = Jump; mort = Die avec rétrécissement;
sortie = Celebrate. Flip X est déjà géré par PlayerMovement.

## 14. Problèmes fréquents

| Problème | Vérification |
|---|---|
| Traverse le sol | Capsule non trigger, Rigidbody2D Dynamic, Default/Ground coché |
| Ne saute pas | Feet trigger au bon endroit, Layer PlayerFeet, tags du sol |
| Inputs inactifs | Asset global assigné, focus dans Game, Console sans erreur |
| Rotation du joueur | Freeze Rotation Z |
| Reste au mur | PlayerMovement attaché, callbacks 2D, vélocité Y conservée |
| Gemme sans points | Tag Gem et collider trigger |
| Bonus à zéro | PlayerBonus attaché et SetBonus appelé sans erreur |
| Sons absents | AudioSource du Player, clips du SoundManager, volume |
| Erreur avec Camera.main | Caméra active taggée MainCamera avec AudioSource |
| Mauvais niveau rechargé | Scene List et ses index |
| Aucun joueur après sortie | Prefab Player absent dans le niveau suivant |
| Sprite derrière le décor | Sorting Layer Player |
| Sprite décalé en animation | Cellules et pivots uniformes |
| Paramètres non conservés | Modification pendant Play ou override non appliqué |

Les sources existantes et les cinq composants obligatoires sont des
prérequis : ne masque pas une référence manquante en ajoutant des tests null
partout. Corrige la configuration et toute erreur rouge de la Console.

## 15. Couverture de l'énoncé et remise

| Tâche | Réalisation |
|---|---|
| 1.1 | Scene List et gravité -17 |
| 1.2 | Construction de Player et matrice des pieds |
| 1.3 | Vélocité horizontale et limites |
| 1.4 | Compteur PlayerGroundDetector |
| 1.5 | Saut immédiat et son, réglages temporaires |
| 1.6 | PlayerGems |
| 1.7 | PlayerBonus |
| 1.8 | PlayerState et GameManager fourni |
| 1.9 | PlayerState, bonus, musique et délai |
| 1.10 | Collision Enter/Stay/Exit et état du mur |
| 1.11 | Charge sans coroutine |
| 1.12 | Coyote time 0.2 |
| 1.13 | Prefab dans les trois niveaux |
| 1.14 | Correction directionnelle déjà incluse |
| 1.15 | Animation optionnelle |

Avant la remise, sauvegarde, enlève les erreurs, remets la gravité à -17,
vérifie les valeurs finales et joue aux trois niveaux depuis le titre.
L'énoncé n'exige pas d'ajouter la sortie avec Escape/Back : garde ce qui est
déjà fourni, sans passer du temps dessus.

Prépare une archive contenant seulement :

```text
PlatformerGame/
    Assets/
    Packages/
    ProjectSettings/
```

Garde tous les `.meta` de ces dossiers. N'inclus ni Library, Temp, Logs, Obj ni
.git. Tu peux faire une copie de remise sans supprimer les dossiers de ton
projet de travail.
Ce guide est à la racine du projet pour consultation; il n'est pas nécessaire
dans l'archive limitée aux trois dossiers.
Le lien Git est aussi autorisé par l'énoncé, s'il est accessible et fonctionnel.

## 16. Validation et ce qu'elle ne garantit pas

Les signatures utilisées ont été vérifiées dans GameManager.cs et
SoundManager.cs, et les paramètres/tags/prefabs ont été examinés.
Les API complémentaires sont sourcées explicitement en section 2.2.

La compilation et les tests des scripts n'ont pas été exécutés. La vérification
approfondie du code est reportée, conformément à ta demande.
Unity n'a pas été lancé : les colliders, sons, animations et parcours à l'écran
n'ont pas été testés.

**Il reste indispensable de faire les tests de la section 11 dans Unity.**
Le guide n'a ajouté aucun script, GameObject, prefab ou paramètre au jeu.
