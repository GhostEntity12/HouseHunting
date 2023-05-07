[1mdiff --git a/Assets/Scripts/InventoryItem.cs b/Assets/Scripts/InventoryItem.cs[m
[1mindex 556980d..4e9e552 100644[m
[1m--- a/Assets/Scripts/InventoryItem.cs[m
[1m+++ b/Assets/Scripts/InventoryItem.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using UnityEngine;[m
[31m-[m
 public struct InventoryItem[m
 {[m
     public string id;[m
[1mdiff --git a/Assets/Scripts/Managers/DecorateManager.cs b/Assets/Scripts/Managers/DecorateManager.cs[m
[1mindex 6a03cc4..384319a 100644[m
[1m--- a/Assets/Scripts/Managers/DecorateManager.cs[m
[1m+++ b/Assets/Scripts/Managers/DecorateManager.cs[m
[36m@@ -1,5 +1,4 @@[m
 using System.Collections.Generic;[m
[31m-using UnityEngine;[m
 [m
 public class DecorateManager : Singleton<DecorateManager>, IDataPersistence[m
 {[m
[1mdiff --git a/Assets/Scripts/Managers/GameManager.cs b/Assets/Scripts/Managers/GameManager.cs[m
[1mindex 4193312..77481c4 100644[m
[1m--- a/Assets/Scripts/Managers/GameManager.cs[m
[1m+++ b/Assets/Scripts/Managers/GameManager.cs[m
[36m@@ -1,5 +1,4 @@[m
 using UnityEngine;[m
[31m-using UnityEngine.SceneManagement;[m
 [m
 public class GameManager : MonoBehaviour, IDataPersistence[m
 {[m
[1mdiff --git a/Assets/Scripts/Player/DecorateInputManager.cs b/Assets/Scripts/Player/DecorateInputManager.cs[m
[1mindex 0421a1f..d35aac3 100644[m
[1m--- a/Assets/Scripts/Player/DecorateInputManager.cs[m
[1m+++ b/Assets/Scripts/Player/DecorateInputManager.cs[m
[36m@@ -1,9 +1,3 @@[m
[31m-using UnityEngine;[m
[31m-using UnityEngine.EventSystems;[m
[31m-using UnityEngine.InputSystem;[m
[31m-using UnityEngine.SceneManagement;[m
[31m-using UnityEngine.UI;[m
[31m-[m
 public class DecorateInputManager : Singleton<DecorateInputManager>[m
 {[m
 [m
[1mdiff --git a/Assets/Scripts/Player/WeaponSwitching.cs b/Assets/Scripts/Player/WeaponSwitching.cs[m
[1mindex 43bf7af..a6b194a 100644[m
[1m--- a/Assets/Scripts/Player/WeaponSwitching.cs[m
[1m+++ b/Assets/Scripts/Player/WeaponSwitching.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
 public class WeaponSwitching : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Shootable/Shootable.cs b/Assets/Scripts/Shootable/Shootable.cs[m
[1mindex ec44daf..d779820 100644[m
[1m--- a/Assets/Scripts/Shootable/Shootable.cs[m
[1m+++ b/Assets/Scripts/Shootable/Shootable.cs[m
[36m@@ -1,5 +1,4 @@[m
 using UnityEngine;[m
[31m-using System.Collections.Generic;[m
 [m
 [m
 public abstract class Shootable : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Singleton.cs b/Assets/Scripts/Singleton.cs[m
[1mindex 304845c..3fb8afa 100644[m
[1m--- a/Assets/Scripts/Singleton.cs[m
[1m+++ b/Assets/Scripts/Singleton.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
 public class Singleton<T> : MonoBehaviour where T : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Spawner.cs b/Assets/Scripts/Spawner.cs[m
[1mindex 21c2814..9e18c1e 100644[m
[1m--- a/Assets/Scripts/Spawner.cs[m
[1m+++ b/Assets/Scripts/Spawner.cs[m
[36m@@ -1,4 +1,3 @@[m
[31m-using System.Collections;[m
 using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
[1mdiff --git a/Assets/Scripts/UI/InventoryItemsUI.cs b/Assets/Scripts/UI/InventoryItemsUI.cs[m
[1mindex a2ee75e..491586d 100644[m
[1m--- a/Assets/Scripts/UI/InventoryItemsUI.cs[m
[1m+++ b/Assets/Scripts/UI/InventoryItemsUI.cs[m
[36m@@ -1,4 +1,3 @@[m
[31m-using TMPro;[m
 using UnityEngine;[m
 using UnityEngine.UI;[m
 [m
[1mdiff --git a/Assets/Scripts/Weapons/Bullet.cs b/Assets/Scripts/Weapons/Bullet.cs[m
[1mindex 98bf430..4b00b13 100644[m
[1m--- a/Assets/Scripts/Weapons/Bullet.cs[m
[1m+++ b/Assets/Scripts/Weapons/Bullet.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
 public class Bullet : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Weapons/BulletHole.cs b/Assets/Scripts/Weapons/BulletHole.cs[m
[1mindex 0453710..150e8e3 100644[m
[1m--- a/Assets/Scripts/Weapons/BulletHole.cs[m
[1m+++ b/Assets/Scripts/Weapons/BulletHole.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
 public class BulletHole : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Weapons/Gun.cs b/Assets/Scripts/Weapons/Gun.cs[m
[1mindex 3e7a0de..d03a348 100644[m
[1m--- a/Assets/Scripts/Weapons/Gun.cs[m
[1m+++ b/Assets/Scripts/Weapons/Gun.cs[m
[36m@@ -1,6 +1,4 @@[m
 using UnityEngine;[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using TMPro;[m
 [m
 public class Gun : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Weapons/MuzzleFlash.cs b/Assets/Scripts/Weapons/MuzzleFlash.cs[m
[1mindex 2933e78..6b04829 100644[m
[1m--- a/Assets/Scripts/Weapons/MuzzleFlash.cs[m
[1m+++ b/Assets/Scripts/Weapons/MuzzleFlash.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
 public class MuzzleFlash : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Weapons/Recoil.cs b/Assets/Scripts/Weapons/Recoil.cs[m
[1mindex f022ccc..c0005ef 100644[m
[1m--- a/Assets/Scripts/Weapons/Recoil.cs[m
[1m+++ b/Assets/Scripts/Weapons/Recoil.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 [m
 public class Recoil : MonoBehaviour[m
[1mdiff --git a/Assets/Scripts/Weapons/WeaponWheelController.cs b/Assets/Scripts/Weapons/WeaponWheelController.cs[m
[1mindex 7646cb9..cac6589 100644[m
[1m--- a/Assets/Scripts/Weapons/WeaponWheelController.cs[m
[1m+++ b/Assets/Scripts/Weapons/WeaponWheelController.cs[m
[36m@@ -1,5 +1,3 @@[m
[31m-using System.Collections;[m
[31m-using System.Collections.Generic;[m
 using UnityEngine;[m
 using UnityEngine.UI;[m
 [m
