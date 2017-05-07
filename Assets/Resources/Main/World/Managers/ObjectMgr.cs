using Assets.Script.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.World.Managers
{
    public class ObjectMgr
    {
        public UInt32 MapID;
        public WoWGuid playerGuid;
        private List<Assets.Scripts.World.Object> mObjects;

        public ObjectMgr()
        {
            mObjects = new List<Assets.Scripts.World.Object>();
        }

        public Assets.Scripts.World.Object getPlayerObject()
        {
            int index = getObjectIndex(playerGuid);
            if (index == -1)
            {
                Assets.Scripts.World.Object obj = new Assets.Scripts.World.Object(playerGuid);
                addObject(obj);
                return obj;

            }
            else
                return mObjects[index];
        }

        public void addObject(Assets.Scripts.World.Object obj)
        {
            Debug.LogWarning("Object created: " + obj.Guid.GetOldGuid());

            int index = getObjectIndex(obj.Guid);
            if (index != -1)
            {
                updateObject(obj);
            }
            else
            {
                mObjects.Add(obj);
                Object[] test = new Object[1];
                test[0] = obj;
            }
        }

        public void updateObject(Object obj)
        {
            Debug.LogWarning("Object updated: " + obj.Guid.GetOldGuid());

            int index = getObjectIndex(obj.Guid);
            if (index != -1)
            {
                mObjects[index] = obj;
                Object[] test = new Object[1];
                test[0] = obj;
            }
            else
            {
                addObject(obj);
            }
        }

        public void delObject(WoWGuid guid)
        {
            int index = getObjectIndex(guid);
            if (index != -1)
            {
                mObjects.RemoveAt(index);
            }
        }

        public Object getObject(string name)
        {
            int index = getObjectIndex(name);
            if (index == -1)
            {
                return null;
            }
            else
                return mObjects[index];
        }

        public Object getObject(WoWGuid guid)
        {
            int index = getObjectIndex(guid);
            if (index == -1)
            {
                return null;
            }
            else
                return mObjects[index];

        }

        public string GetClassName(int i)
        {
            Classname Class = (Classname)i;

            string className = "";

            switch (Class)
            {
                case Classname.Druid:
                    className = "Druid";
                    break;
                case Classname.Hunter:
                    className = "Hunter";
                    break;
                case Classname.Mage:
                    className = "Mage";
                    break;
                case Classname.Paladin:
                    className = "Paladin";
                    break;
                case Classname.Priest:
                    className = "Priest";
                    break;
                case Classname.Rogue:
                    className = "Rogue";
                    break;
                case Classname.Shaman:
                    className = "Shaman";
                    break;
                case Classname.Warlock:
                    className = "Warlock";
                    break;
                case Classname.Warrior:
                    className = "Warrior";
                    break;
            }

            return className;
        }

        public Object getNearestObject(Object obj)
        {
            Object[] list = getObjectArray();
            Object closest = null;
            float dist;
            float mindist = 9999999999;

            if (list.Length < 1)
            {
                return null;
            }

            foreach (Object obj2 in list)
            {
                dist = TerrainMgr.CalculateDistance(obj.Position, obj2.Position);
                if (dist < mindist)
                {
                    mindist = dist;
                    closest = obj2;
                }
            }

            return closest;
        }

        public Object getNearestObject()
        {
            Object[] list = getObjectArray();
            Object closest = null;
            float dist;
            float mindist = 9999999999;

            if (list.Length < 1)
            {
                return null;
            }

            foreach (Object obj2 in list)
            {
                if (obj2.Guid.GetOldGuid() != playerGuid.GetOldGuid())
                {
                    dist = TerrainMgr.CalculateDistance(getPlayerObject().Position, obj2.Position);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closest = obj2;
                    }
                }
            }

            return closest;
        }

        public ObjectType getObjectType(WoWGuid guid)
        {
            int index = getObjectIndex(guid);
            if (index != -1)
            {
                return mObjects[index].Type;
            }
            else
                return new ObjectType();
        }

        public bool objectExists(WoWGuid guid)
        {

            int index = getObjectIndex(guid);
            if (index == -1)
            {
                return false;
            }
            else
                return true;
        }

        private int getObjectIndex(WoWGuid guid)
        {
            int index = mObjects.FindIndex(s => s.Guid.GetOldGuid() == guid.GetOldGuid());
            return index;
        }

        private int getObjectIndex(string name)
        {
            int index = mObjects.FindIndex(s => s.Name == name);
            return index;
        }

        public Object[] getObjectArray()
        {
            return mObjects.ToArray();
        }

        public string GetZoneByID(int zone)
        {
            string zoneName = "";

            switch (zone)
            {
                case 1:
                    zoneName = "Dun Morogh";
                    break;
                case 215:
                    zoneName = "Mulgore";
                    break;
                    
/*2 Longshore = 2
3 Badlands = 3
4 Blasted Lands = 4
7 Blackwater Cove = 7
8 Swamp of Sorrows = 8
9 Northshire Valley = 9
10 Duskwood = 10
11 Wetlands = 11
12 Elwynn Forest = 12
13 The World Tree = 13
14 Durotar = 14
15 Dustwallow Marsh = 15
16 Azshara = 16
17 The Barrens = 17
18 Crystal Lake = 18
19 Zul'Gurub =19 
20 Moonbrook = 20
21 Kul Tiras = 21
22 Programmer Isle = 22
23 Northshire River = 23
24 Northshire Abbey = 24
25 Blackrock Mountain = 25
26 Lighthouse = 26
28 Western Plaguelands = 28
30 Nine = 30
32 The Cemetary = 32
33 Stranglethorn Vale = 33
34 Echo Ridge Mine = 34
35 Booty Bay = 35
36 Alterac Mountains = 36
37 Lake Nazferiti = 37
38 Loch Modan = 38
40 Westfall = 40
41 Deadwind Pass = 41
42 Darkshire = 42
43 Wild Shore = 43
44 Redridge Mountains = 44
45 Arathi Highlands = 45
46 Burning Steppes = 46
47 The Hinterlands = 47
49 Dead Man's Hole =49 
51 Searing Gorge = 51
53 Thieves Camp = 53
54 Jasperlode Mine = 54
55 Valley of Heroes = 55
56 Heroes' Vigil =56 
57 Fargodeep Mine = 57
59 Northshire Vineyards = 59
60 Forest's Edge =60 
61 Thunder Falls = 61
62 Brackwell Pumpkin Patch = 62
63 The Stonefield Farm = 63
64 The Maclure Vineyards = 64
68 Lake Everstill = 68
69 Lakeshire = 69
70 Stonewatch = 70
71 Stonewatch Falls = 71
72 The Dark Portal = 72
73 The Tainted Scar = 73
74 Pool of Tears = 74
75 Stonard = 75
76 Fallow Sanctuary = 76
77 Anvilmar = 77
80 Stormwind Mountains = 80
85 Tirisfal Glades = 85
86 Stone Cairn Lake = 86
87 Goldshire = 87
88 Eastvale Logging Camp = 88
89 Mirror Lake Orchard = 89
91 Tower of Azora = 91
92 Mirror Lake = 92
93 Vul'Gol Ogre Mound =93 
94 Raven Hill = 94
95 Redridge Canyons = 95
96 Tower of Ilgalar = 96
97 Alther's Mill =97 
98 Rethban Caverns = 98
99 Rebel Camp = 99
100 Nesingwary's Expedition =100 
101 Kurzen's Compound =101 
102 Ruins of Zul'Kunda =102 
103 Ruins of Zul'Mamwe =103 
104 The Vile Reef = 104
105 Mosh'Ogg Ogre Mound =105 
106 The Stockpile = 106
107 Saldean's Farm =107 
108 Sentinel Hill = 108
109 Furlbrow's Pumpkin Farm =109 
111 Jangolode Mine = 111
113 Gold Coast Quarry = 113
115 Westfall Lighthouse = 115
116 Misty Valley = 116
117 Grom'gol Base Camp =117 
118 Whelgar's Excavation Site =118 
120 Westbrook Garrison = 120
121 Tranquil Gardens Cemetery = 121
122 Zuuldaia Ruins = 122
123 Bal'lal Ruins =123 
125 Kal'ai Ruins =125 
126 Tkashi Ruins = 126
127 Balia'mah Ruins =127 
128 Ziata'jai Ruins =128 
129 Mizjah Ruins = 129
130 Silverpine Forest = 130
131 Kharanos = 131
132 Coldridge Valley = 132
133 Gnomeregan = 133
134 Gol'Bolar Quarry =134 
135 Frostmane Hold = 135
136 The Grizzled Den = 136
137 Brewnall Village = 137
138 Misty Pine Refuge = 138
139 Eastern Plaguelands = 139
141 Teldrassil = 141
142 Ironband's Excavation Site =142 
143 Mo'grosh Stronghold =143 
144 Thelsamar = 144
145 Algaz Gate = 145
146 Stonewrought Dam = 146
147 The Farstrider Lodge = 147
148 Darkshore = 148
149 Silver Stream Mine = 149
150 Menethil Harbor = 150
151 Designer Island = 151
152 The Bulwark = 152
153 Ruins of Lordaeron = 153
154 Deathknell = 154
155 Night Web's Hollow =155 
156 Solliden Farmstead = 156
157 Agamand Mills = 157
158 Agamand Family Crypt = 158
159 Brill = 159
160 Whispering Gardens = 160
161 Terrace of Repose = 161
162 Brightwater Lake = 162
163 Gunther's Retreat =163 
164 Garren's Haunt =164 
165 Balnir Farmstead = 165
166 Cold Hearth Manor = 166
167 Crusader Outpost = 167
168 The North Coast = 168
169 Whispering Shore = 169
170 Lordamere Lake = 170
172 Fenris Isle = 172
173 Faol's Rest =173 
186 Dolanaar = 186
188 Shadowglen = 188
189 Steelgrill's Depot =189 
190 Hearthglen = 190
192 Northridge Lumber Camp = 192
193 Ruins of Andorhal = 193
195 School of Necromancy = 195
196 Uther's Tomb =196 
197 Sorrow Hill = 197
198 The Weeping Cave = 198
199 Felstone Field = 199
200 Dalson's Tears =200 
201 Gahrron's Withering =201 
202 The Writhing Haunt = 202
203 Mardenholde Keep = 203
204 Pyrewood Village = 204
205 Dun Modr = 205
207 The Great Sea = 207
208 Unused Ironcladcove = 208
209 Shadowfang Keep = 209
211 Iceflow Lake = 211
212 Helm's Bed Lake =212 
213 Deep Elem Mine = 213
214 The Great Sea = 214
215 Mulgore = 215
219 Alexston Farmstead = 219
220 Red Cloud Mesa = 220
221 Camp Narache = 221
222 Bloodhoof Village = 222
223 Stonebull Lake = 223
224 Ravaged Caravan = 224
225 Red Rocks = 225
226 The Skittering Dark = 226
227 Valgan's Field =227 
228 The Sepulcher = 228
229 Olsen's Farthing =229 
230 The Greymane Wall = 230
231 Beren's Peril =231 
232 The Dawning Isles = 232
233 Ambermill = 233
235 Fenris Keep = 235
236 Shadowfang Keep = 236
237 The Decrepit Ferry = 237
238 Malden's Orchard =238 
239 The Ivar Patch = 239
240 The Dead Field = 240
241 The Rotting Orchard = 241
242 Brightwood Grove = 242
243 Forlorn Rowe = 243
244 The Whipple Estate = 244
245 The Yorgen Farmstead = 245
246 The Cauldron = 246
247 Grimesilt Dig Site = 247
249 Dreadmaul Rock = 249
250 Ruins of Thaurissan = 250
251 Flame Crest = 251
252 Blackrock Stronghold = 252
253 The Pillar of Ash = 253
254 Blackrock Mountain = 254
255 Altar of Storms = 255
256 Aldrassil = 256
257 Shadowthread Cave = 257
258 Fel Rock = 258
259 Lake Al'Ameth =259 
260 Starbreeze Village = 260
261 Gnarlpine Hold = 261
262 Ban'ethil Barrow Den =262 
263 The Cleft = 263
264 The Oracle Glade = 264
265 Wellspring River = 265
266 Wellspring Lake = 266
267 Hillsbrad Foothills = 267
268 Azshara Crater = 268
269 Dun Algaz = 269
271 Southshore = 271
272 Tarren Mill = 272
275 Durnholde Keep = 275
276 Stonewrought Pass = 276
277 The Foothill Caverns = 277
278 Lordamere Internment Camp = 278
279 Dalaran = 279
280 Strahnbrad = 280
281 Ruins of Alterac = 281
282 Crushridge Hold = 282
283 Slaughter Hollow = 283
284 The Uplands = 284
285 Southpoint Tower = 285
286 Hillsbrad Fields = 286
287 Hillsbrad = 287
288 Azurelode Mine = 288
289 Nethander Stead = 289
290 Dun Garok = 290
293 Thoradin's Wall =293 
294 Eastern Strand = 294
295 Western Strand = 295
297 Jaguero Isle = 297
298 Baradin Bay = 298
299 Menethil Bay = 299
300 Misty Reed Strand = 300
301 The Savage Coast = 301
302 The Crystal Shore = 302
303 Shell Beach = 303
305 North Tide's Run =305 
306 South Tide's Run =306 
307 The Overlook Cliffs = 307
308 The Forbidding Sea = 308
309 Ironbeard's Tomb =309 
310 Crystalvein Mine = 310
311 Ruins of Aboraz = 311
312 Janeiro's Point =312 
313 Northfold Manor = 313
314 Go'Shek Farm =314 
315 Dabyrie's Farmstead =315 
316 Boulderfist Hall = 316
317 Witherbark Village = 317
318 Drywhisker Gorge = 318
320 Refuge Pointe = 320
321 Hammerfall = 321
322 Blackwater Shipwrecks = 322
323 O'Breen's Camp = 323
324 Stromgarde Keep = 324
325 The Tower of Arathor = 325
326 The Sanctum = 326
327 Faldir's Cove =327 
328 The Drowned Reef = 328
330 Thandol Span = 330
331 Ashenvale = 331
332 The Great Sea = 332
333 Circle of East Binding = 333
334 Circle of West Binding = 334
335 Circle of Inner Binding = 335
336 Circle of Outer Binding = 336
337 Apocryphan's Rest =337 
338 Angor Fortress = 338
339 Lethlor Ravine = 339
340 Kargath = 340
341 Camp Kosh = 341
342 Camp Boff = 342
343 Camp Wurg = 343
344 Camp Cagg = 344
345 Agmond's End =345 
346 Hammertoe's Digsite =346 
347 Dustbelch Grotto = 347
348 Aerie Peak = 348
349 Wildhammer Keep = 349
350 Quel'Danil Lodge =350 
351 Skulk Rock = 351
352 Zun'watha =352 
353 Shadra'Alor =353 
354 Jintha'Alor =354 
355 The Altar of Zul = 355
356 Seradane = 356
357 Feralas = 357
358 Brambleblade Ravine = 358
359 Bael Modan = 359
360 The Venture Co.Mine = 360
361 Felwood = 361
362 Razor Hill = 362
363 Valley of Trials = 363
364 The Den = 364
365 Burning Blade Coven = 365
366 Kolkar Crag = 366
367 Sen'jin Village =367 
368 Echo Isles = 368
369 Thunder Ridge = 369
370 Drygulch Ravine = 370
371 Dustwind Cave = 371
372 Tiragarde Keep = 372
373 Scuttle Coast = 373
374 Bladefist Bay = 374
375 Deadeye Shore = 375
377 Southfury River = 377
378 Camp Taurajo = 378
379 Far Watch Post = 379
380 The Crossroads = 380
381 Boulder Lode Mine = 381
382 The Sludge Fen = 382
383 The Dry Hills = 383
384 Dreadmist Peak = 384
385 Northwatch Hold = 385
386 The Forgotten Pools = 386
387 Lushwater Oasis = 387
388 The Stagnant Oasis = 388
390 Field of Giants = 390
391 The Merchant Coast = 391
392 Ratchet = 392
393 Darkspear Strand = 393
396 Winterhoof Water Well = 396
397 Thunderhorn Water Well = 397
398 Wildmane Water Well = 398
399 Skyline Ridge = 399
400 Thousand Needles = 400
401 The Tidus Stair = 401
403 Shady Rest Inn = 403
404 Bael'dun Digsite =404 
405 Desolace = 405
406 Stonetalon Mountains = 406
408 Gillijim's Isle =408 
409 Island of Doctor Lapidis = 409
410 Razorwind Canyon = 410
411 Bathran's Haunt =411 
412 The Ruins of Ordil'Aran =412 
413 Maestra's Post =413 
414 The Zoram Strand = 414
415 Astranaar = 415
416 The Shrine of Aessina = 416
417 Fire Scar Shrine = 417
418 The Ruins of Stardust = 418
419 The Howling Vale = 419
420 Silverwind Refuge = 420
421 Mystral Lake = 421
422 Fallen Sky Lake = 422
424 Iris Lake = 424
425 Moonwell = 425
426 Raynewood Retreat = 426
427 The Shady Nook = 427
428 Night Run = 428
429 Xavian = 429
430 Satyrnaar = 430
431 Splintertree Post = 431
432 The Dor'Danil Barrow Den =432 
433 Falfarren River = 433
434 Felfire Hill = 434
435 Demon Fall Canyon = 435
436 Demon Fall Ridge = 436
437 Warsong Lumber Camp = 437
438 Bough Shadow = 438
439 The Shimmering Flats = 439
440 Tanaris = 440
441 Lake Falathim = 441
442 Auberdine = 442
443 Ruins of Mathystra = 443
444 Tower of Althalaxx = 444
445 Cliffspring Falls = 445
446 Bashal'Aran =446 
447 Ameth'Aran =447 
448 Grove of the Ancients = 448
449 The Master's Glaive =449 
450 Remtravel's Excavation =450 
452 Mist's Edge =452 
453 The Long Wash = 453
454 Wildbend River = 454
455 Blackwood Den = 455
456 Cliffspring River = 456
457 The Veiled Sea = 457
458 Gold Road = 458
459 Scarlet Watch Post = 459
460 Sun Rock Retreat = 460
461 Windshear Crag = 461
463 Cragpool Lake = 463
464 Mirkfallon Lake = 464
465 The Charred Vale = 465
466 Valley of the Bloodfuries = 466
467 Stonetalon Peak = 467
468 The Talon Den = 468
469 Greatwood Vale = 469
471 Brave Wind Mesa = 471
472 Fire Stone Mesa = 472
473 Mantle Rock = 473
477 Ruins of Jubuwal = 477
478 Pools of Arlithrien = 478
479 The Rustmaul Dig Site = 479
480 Camp E'thok =480 
481 Splithoof Crag = 481
482 Highperch = 482
483 The Screeching Canyon = 483
484 Freewind Post = 484
485 The Great Lift = 485
486 Galak Hold = 486
487 Roguefeather Den = 487
488 The Weathered Nook = 488
489 Thalanaar = 489
490 Un'Goro Crater =490 
491 Razorfen Kraul = 491
492 Raven Hill Cemetery = 492
493 Moonglade = 493
496 Brackenwall Village = 496
497 Swamplight Manor = 497
498 Bloodfen Burrow = 498
499 Darkmist Cavern = 499
500 Moggle Point = 500
501 Beezil's Wreck =501 
502 Witch Hill = 502
503 Sentry Point = 503
504 North Point Tower = 504
505 West Point Tower = 505
506 Lost Point = 506
507 Bluefen = 507
508 Stonemaul Ruins = 508
509 The Den of Flame = 509
510 The Dragonmurk = 510
511 Wyrmbog = 511
512 Onyxia's Lair =512 
513 Theramore Isle = 513
514 Foothold Citadel = 514
515 Ironclad Prison = 515
516 Dustwallow Bay = 516
517 Tidefury Cove = 517
518 Dreadmurk Shore = 518
536 Addle's Stead =536 
537 Fire Plume Ridge = 537
538 Lakkari Tar Pits = 538
539 Terror Run = 539
540 The Slithering Scar = 540
541 Marshal's Refuge =541 
542 Fungal Rock = 542
543 Golakka Hot Springs = 543
556 The Loch = 556
576 Beggar's Haunt =576 
596 Kodo Graveyard = 596
597 Ghost Walker Post = 597
598 Sar'theris Strand =598 
599 Thunder Axe Fortress = 599
600 Bolgan's Hole =600 
602 Mannoroc Coven = 602
603 Sargeron = 603
604 Magram Village = 604
606 Gelkis Village = 606
607 Valley of Spears = 607
608 Nijel's Point =608 
609 Kolkar Village = 609
616 Hyjal = 616
618 Winterspring = 618
636 Blackwolf River = 636
637 Kodo Rock = 637
638 Hidden Path = 638
639 Spirit Rock = 639
640 Shrine of the Dormant Flame = 640
656 Lake Elune'ara =656 
657 The Harborage = 657
676 Outland = 676
696 Craftsmen's Terrace =696 
697 Tradesmen's Terrace =697 
698 The Temple Gardens = 698
699 Temple of Elune = 699
700 Cenarion Enclave = 700
701 Warrior's Terrace =701 
702 Rut'theran Village =702 
716 Ironband's Compound =716 
717 The Stockade = 717
718 Wailing Caverns = 718
719 Blackfathom Deeps = 719
720 Fray Island = 720
721 Gnomeregan = 721
722 Razorfen Downs = 722
736 Ban'ethil Hollow =736 
796 Scarlet Monastery = 796
797 Jerod's Landing =797 
798 Ridgepoint Tower = 798
799 The Darkened Bank = 799
800 Coldridge Pass = 800
801 Chill Breeze Valley = 801
802 Shimmer Ridge = 802
803 Amberstill Ranch = 803
804 The Tundrid Hills = 804
805 South Gate Pass = 805
806 South Gate Outpost = 806
807 North Gate Pass = 807
808 North Gate Outpost = 808
809 Gates of Ironforge = 809
810 Stillwater Pond = 810
811 Nightmare Vale = 811
812 Venomweb Vale = 812
813 The Bulwark = 813
814 Southfury River = 814
815 Southfury River = 815
816 Razormane Grounds = 816
817 Skull Rock = 817
818 Palemane Rock = 818
819 Windfury Ridge = 819
820 The Golden Plains = 820
821 The Rolling Plains = 821
836 Dun Algaz = 836
837 Dun Algaz = 837
838 North Gate Pass = 838
839 South Gate Pass = 839
856 Twilight Grove = 856
876 GM Island = 876
878 Southfury River = 878
879 Southfury River = 879
880 Thandol Span = 880
881 Thandol Span = 881
896 Purgation Isle = 896
916 The Jansen Stead = 916
917 The Dead Acre = 917
918 The Molsen Farm = 918
919 Stendel's Pond =919 
920 The Dagger Hills = 920
921 Demont's Place =921 
922 The Dust Plains = 922
923 Stonesplinter Valley = 923
924 Valley of Kings = 924
925 Algaz Station = 925
926 Bucklebree Farm = 926
927 The Shining Strand = 927
928 North Tide's Hollow =928 
936 Grizzlepaw Ridge = 936
956 The Verdant Fields = 956
976 Gadgetzan = 976
977 Steamwheedle Port = 977
978 Zul'Farrak =978 
979 Sandsorrow Watch = 979
980 Thistleshrub Valley = 980
981 The Gaping Chasm = 981
982 The Noxious Lair = 982
983 Dunemaul Compound = 983
984 Eastmoon Ruins = 984
985 Waterspring Field = 985
986 Zalashji's Den =986 
987 Land's End Beach =987 
988 Wavestrider Beach = 988
989 Uldum = 989
990 Valley of the Watchers = 990
991 Gunstan's Post =991 
992 Southmoon Ruins = 992
996 Render's Camp =996 
997 Render's Valley =997 
998 Render's Rock =998 
999 Stonewatch Tower = 999
1000 Galardell Valley = 1000
1001 Lakeridge Highway = 1001
1002 Three Corners = 1002
1016 Direforge Hill = 1016
1017 Raptor Ridge = 1017
1018 Black Channel Marsh = 1018
1019 The Green Belt = 1019
1020 Mosshide Fen = 1020
1021 Thelgen Rock = 1021
1022 Bluegill Marsh = 1022
1023 Saltspray Glen = 1023
1024 Sundown Marsh = 1024
1025 The Green Belt = 1025
1036 Angerfang Encampment = 1036
1037 Grim Batol = 1037
1038 Dragonmaw Gates = 1038
1039 The Lost Fleet = 1039
1056 Darrow Hill = 1056
1057 Thoradin's Wall =1057
1076 Webwinder Path = 1076
1097 The Hushed Bank = 1097
1098 Manor Mistmantle = 1098
1099 Camp Mojache = 1099
1100 Grimtotem Compound = 1100
1101 The Writhing Deep = 1101
1102 Wildwind Lake = 1102
1103 Gordunni Outpost = 1103
1104 Mok'Gordun =1104
1105 Feral Scar Vale = 1105
1106 Frayfeather Highlands = 1106
1107 Idlewind Lake = 1107
1108 The Forgotten Coast = 1108
1109 East Pillar = 1109
1110 West Pillar = 1110
1111 Dream Bough = 1111
1112 Jademir Lake = 1112
1113 Oneiros = 1113
1114 Ruins of Ravenwind = 1114
1115 Rage Scar Hold = 1115
1116 Feathermoon Stronghold = 1116
1117 Ruins of Solarsal = 1117
1119 The Twin Colossals = 1119
1120 Sardor Isle = 1120
1121 Isle of Dread = 1121
1136 High Wilderness = 1136
1137 Lower Wilds = 1137
1156 Southern Barrens = 1156
1157 Southern Gold Road = 1157
1176 Zul'Farrak =1176
1216 Timbermaw Hold = 1216
1217 Vanndir Encampment = 1217
1219 Legash Encampment = 1219
1220 Thalassian Base Camp = 1220
1221 Ruins of Eldarath = 1221
1222 Hetaera's Clutch =1222
1223 Temple of Zin - Malor = 1223
1224 Bear's Head =1224
1225 Ursolan = 1225
1226 Temple of Arkkoran = 1226
1227 Bay of Storms = 1227
1228 The Shattered Strand = 1228
1229 Tower of Eldara = 1229
1230 Jagged Reef = 1230
1231 Southridge Beach = 1231
1232 Ravencrest Monument = 1232
1233 Forlorn Ridge = 1233
1234 Lake Mennar = 1234
1235 Shadowsong Shrine = 1235
1236 Haldarr Encampment = 1236
1237 Valormok = 1237
1256 The Ruined Reaches = 1256
1276 The Talondeep Path = 1276
1277 The Talondeep Path = 1277
1296 Rocktusk Farm = 1296
1297 Jaggedswine Farm = 1297
1316 Razorfen Downs = 1316
1336 Lost Rigger Cove = 1336
1337 Uldaman = 1337
1338 Lordamere Lake = 1338
1339 Lordamere Lake = 1339
1357 Gallows' Corner =1357
1377 Silithus = 1377
1397 Emerald Forest = 1397
1417 Sunken Temple = 1417
1437 Dreadmaul Hold = 1437
1438 Nethergarde Keep = 1438
1439 Dreadmaul Post = 1439
1440 Serpent's Coil =1440
1441 Altar of Storms = 1441
1442 Firewatch Ridge = 1442
1443 The Slag Pit = 1443
1444 The Sea of Cinders = 1444
1445 Blackrock Mountain = 1445
1446 Thorium Point = 1446
1457 Garrison Armory = 1457
1477 The Temple of Atal'Hakkar =1477
1497 Undercity = 1497
1517 Uldaman = 1517
1518 Not Used Deadmines = 1518
1519 Stormwind City = 1519
1537 Ironforge = 1537
1557 Splithoof Hold = 1557
1577 The Cape of Stranglethorn = 1577
1578 Southern Savage Coast = 1578
1579 Unused The Deadmines 002 = 1579
1580 Unused Ironclad Cove 003 = 1580
1581 The Deadmines = 1581
1582 Ironclad Cove = 1582
1583 Blackrock Spire = 1583
1584 Blackrock Depths = 1584
1597 Raptor Grounds UNUSED = 1597
1598 Grol'dom Farm UNUSED =1598
1599 Mor'shan Base Camp =1599
1600 Honor's Stand UNUSED =1600
1601 Blackthorn Ridge UNUSED = 1601
1602 Bramblescar UNUSED = 1602
1603 Agama'gor UNUSED =1603
1617 Valley of Heroes = 1617
1637 Orgrimmar = 1637
1638 Thunder Bluff = 1638
1639 Elder Rise = 1639
1640 Spirit Rise = 1640
1641 Hunter Rise = 1641
1657 Darnassus = 1657
1658 Cenarion Enclave = 1658
1659 Craftsmen's Terrace =1659
1660 Warrior's Terrace =1660
1661 The Temple Gardens = 1661
1662 Tradesmen's Terrace =1662
1677 Gavin's Naze =1677
1678 Sofera's Naze =1678
1679 Corrahn's Dagger =1679
1680 The Headland = 1680
1681 Misty Shore = 1681
1682 Dandred's Fold =1682
1683 Growless Cave = 1683
1684 Chillwind Point = 1684
1697 Raptor Grounds = 1697
1698 Bramblescar = 1698
1699 Thorn Hill = 1699
1700 Agama'gor =1700
1701 Blackthorn Ridge = 1701
1702 Honor's Stand =1702
1703 The Mor'shan Rampart =1703
1704 Grol'dom Farm =1704
1717 Razorfen Kraul = 1717
1718 The Great Lift = 1718
1737 Mistvale Valley = 1737
1738 Nek'mani Wellspring =1738
1739 Bloodsail Compound = 1739
1740 Venture Co. Base Camp = 1740
1741 Gurubashi Arena = 1741
1742 Spirit Den = 1742
1757 The Crimson Veil = 1757
1758 The Riptide = 1758
1759 The Damsel's Luck =1759
1760 Venture Co. Operations Center = 1760
1761 Deadwood Village = 1761
1762 Felpaw Village = 1762
1763 Jaedenar = 1763
1764 Bloodvenom River = 1764
1765 Bloodvenom Falls = 1765
1766 Shatter Scar Vale = 1766
1767 Irontree Woods = 1767
1768 Irontree Cavern = 1768
1769 Timbermaw Hold = 1769
1770 Shadow Hold = 1770
1771 Shrine of the Deceiver = 1771
1777 Itharius's Cave =1777
1778 Sorrowmurk = 1778
1779 Draenil'dur Village =1779
1780 Splinterspear Junction = 1780
1797 Stagalbog = 1797
1798 The Shifting Mire = 1798
1817 Stagalbog Cave = 1817
1837 Witherbark Caverns = 1837
1857 Thoradin's Wall =1857
1858 Boulder'gor =1858
1877 Valley of Fangs = 1877
1878 The Dustbowl = 1878
1879 Mirage Flats = 1879
1880 Featherbeard's Hovel =1880
1881 Shindigger's Camp =1881
1882 Plaguemist Ravine = 1882
1883 Valorwind Lake = 1883
1884 Agol'watha =1884
1885 Hiri'watha =1885
1886 The Creeping Ruin = 1886
1887 Bogen's Ledge =1887
1897 The Maker's Terrace =1897
1898 Dustwind Gulch = 1898
1917 Shaol'watha =1917
1937 Noonshade Ruins = 1937
1938 Broken Pillar = 1938
1939 Abyssal Sands = 1939
1940 Southbreak Shore = 1940
1941 Caverns of Time = 1941
1942 The Marshlands = 1942
1943 Ironstone Plateau = 1943
1957 Blackchar Cave = 1957
1958 Tanner Camp = 1958
1959 Dustfire Valley = 1959
1977 Zul'Gurub =1977
1978 Misty Reed Post = 1978
1997 Bloodvenom Post = 1997
1998 Talonbranch Glade = 1998
2017 Stratholme = 2017
2037 Quel'thalas =2037
2057 Scholomance = 2057
2077 Twilight Vale = 2077
2078 Twilight Shore = 2078
2079 Alcaz Island = 2079
2097 Darkcloud Pinnacle = 2097
2098 Dawning Wood Catacombs = 2098
2099 Stonewatch Keep = 2099
2100 Maraudon = 2100
2101 Stoutlager Inn = 2101
2102 Thunderbrew Distillery = 2102
2103 Menethil Keep = 2103
2104 Deepwater Tavern = 2104
2117 Shadow Grave = 2117
2118 Brill Town Hall = 2118
2119 Gallows' End Tavern =2119
2137 The Pools of Vision = 2137
2138 Dreadmist Den = 2138
2157 Bael'dun Keep =2157
2158 Emberstrife's Den =2158
2159 Onyxia's Lair =2159
2160 Windshear Mine = 2160
2161 Roland's Doom =2161
2177 Battle Ring = 2177
2197 The Pools of Vision = 2197
2198 Shadowbreak Ravine = 2198
2217 Broken Spear Village = 2217
2237 Whitereach Post = 2237
2238 Gornia = 2238
2239 Zane's Eye Crater =2239
2240 Mirage Raceway = 2240
2241 Frostsaber Rock = 2241
2242 The Hidden Grove = 2242
2243 Timbermaw Post = 2243
2244 Winterfall Village = 2244
2245 Mazthoril = 2245
2246 Frostfire Hot Springs = 2246
2247 Ice Thistle Hills = 2247
2248 Dun Mandarr = 2248
2249 Frostwhisper Gorge = 2249
2250 Owl Wing Thicket = 2250
2251 Lake Kel'Theril =2251
2252 The Ruins of Kel'Theril =2252
2253 Starfall Village = 2253
2254 Ban'Thallow Barrow Den =2254
2255 Everlook = 2255
2256 Darkwhisper Gorge = 2256
2257 Deeprun Tram = 2257
2258 The Fungal Vale = 2258
2259 The Marris Stead = 2259
2260 The Marris Stead = 2260
2261 The Undercroft = 2261
2262 Darrowshire = 2262
2263 Crown Guard Tower = 2263
2264 Corin's Crossing =2264
2265 Scarlet Base Camp = 2265
2266 Tyr's Hand =2266
2267 The Scarlet Basilica = 2267
2268 Light's Hope Chapel =2268
2269 Browman Mill = 2269
2270 The Noxious Glade = 2270
2271 Eastwall Tower = 2271
2272 Northdale = 2272
2273 Zul'Mashar =2273
2274 Mazra'Alor =2274
2275 Northpass Tower = 2275
2276 Quel'Lithien Lodge =2276
2277 Plaguewood = 2277
2278 Scourgehold = 2278
2279 Stratholme = 2279
2280 Stratholme = 2280
2297 Darrowmere Lake = 2297
2298 Caer Darrow = 2298
2299 Darrowmere Lake = 2299
2300 Caverns of Time = 2300
2301 Thistlefur Village = 2301
2302 The Quagmire = 2302
2303 Windbreak Canyon = 2303
2317 South Seas = 2317
2318 The Great Sea = 2318
2319 The Great Sea = 2319
2320 The Great Sea = 2320
2321 The Great Sea = 2321
2322 The Veiled Sea = 2322
2323 The Veiled Sea = 2323
2324 The Veiled Sea = 2324
2325 The Veiled Sea = 2325
2326 The Veiled Sea = 2326
2337 Razor Hill Barracks = 2337
2338 South Seas = 2338
2339 The Great Sea = 2339
2357 Bloodtooth Camp = 2357
2358 Forest Song = 2358
2359 Greenpaw Village = 2359
2360 Silverwing Outpost = 2360
2361 Nighthaven = 2361
2362 Shrine of Remulos = 2362
2363 Stormrage Barrow Dens = 2363
2364 The Great Sea = 2364
2365 The Great Sea = 2365
2366 The Black Morass = 2366
2367 Old Hillsbrad Foothills = 2367
2368 Tarren Mill = 2368
2369 Southshore = 2369
2370 Durnholde Keep = 2370
2371 Dun Garok = 2371
2372 Hillsbrad Fields = 2372
2373 Eastern Strand = 2373
2374 Nethander Stead = 2374
2375 Darrow Hill = 2375
2376 Southpoint Tower = 2376
2377 Thoradin's Wall =2377
2378 Western Strand = 2378
2379 Azurelode Mine = 2379
2397 The Great Sea = 2397
2398 The Great Sea = 2398
2399 The Great Sea = 2399
2400 The Forbidding Sea = 2400
2401 The Forbidding Sea = 2401
2402 The Forbidding Sea = 2402
2403 The Forbidding Sea = 2403
2404 Tethris Aran = 2404
2405 Ethel Rethor = 2405
2406 Ranazjar Isle = 2406
2407 Kormek's Hut =2407
2408 Shadowprey Village = 2408
2417 Blackrock Pass = 2417
2418 Morgan's Vigil =2418
2419 Slither Rock = 2419
2420 Terror Wing Path = 2420
2421 Draco'dar =2421
2437 Ragefire Chasm = 2437
2457 Nightsong Woods = 2457
2477 The Veiled Sea = 2477
2478 Morlos'Aran =2478
2479 Emerald Sanctuary = 2479
2480 Jadefire Glen = 2480
2481 Ruins of Constellas = 2481
2497 Bitter Reaches = 2497
2517 Rise of the Defiler = 2517
2518 Lariss Pavilion = 2518
2519 Woodpaw Hills = 2519
2520 Woodpaw Den = 2520
2521 Verdantis River = 2521
2522 Ruins of Isildien = 2522
2537 Grimtotem Post = 2537
2538 Camp Aparaje = 2538
2539 Malaka'jin =2539
2540 Boulderslide Ravine = 2540
2541 Sishir Canyon = 2541
2557 Dire Maul = 2557
2558 Deadwind Ravine = 2558
2559 Diamondhead River = 2559
2560 Ariden's Camp =2560
2561 The Vice = 2561
2562 Karazhan = 2562
2563 Morgan's Plot =2563
2577 Dire Maul = 2577
2597 Alterac Valley = 2597
2617 Scrabblescrew's Camp =2617
2618 Jadefire Run = 2618
2619 Thondroril River = 2619
2620 Thondroril River = 2620
2621 Lake Mereldar = 2621
2622 Pestilent Scar = 2622
2623 The Infectis Scar = 2623
2624 Blackwood Lake = 2624
2625 Eastwall Gate = 2625
2626 Terrorweb Tunnel = 2626
2627 Terrordale = 2627
2637 Kargathia Keep = 2637
2657 Valley of Bones = 2657
2677 Blackwing Lair = 2677
2697 Deadman's Crossing =2697
2717 Molten Core = 2717
2737 The Scarab Wall = 2737
2738 Southwind Village = 2738
2739 Twilight Base Camp = 2739
2740 The Crystal Vale = 2740
2741 The Scarab Dais = 2741
2742 Hive'Ashi =2742
2743 Hive'Zora =2743
2744 Hive'Regal =2744
2757 Shrine of the Fallen Warrior = 2757
2777 Alterac Valley = 2777
2797 Blackfathom Deeps = 2797
2837 The Master's Cellar =2837
2838 Stonewrought Pass = 2838
2839 Alterac Valley = 2839
2857 The Rumble Cage = 2857
2877 Chunk Test = 2877
2897 Zoram'gar Outpost =2897
2917 Hall of Legends = 2917
2918 Champions' Hall =2918
2937 Grosh'gok Compound =2937
2938 Sleeping Gorge = 2938
2957 Irondeep Mine = 2957
2958 Stonehearth Outpost = 2958
2959 Dun Baldar = 2959
2960 Icewing Pass = 2960
2961 Frostwolf Village = 2961
2962 Tower Point = 2962
2963 Coldtooth Mine = 2963
2964 Winterax Hold = 2964
2977 Iceblood Garrison = 2977
2978 Frostwolf Keep = 2978
2979 Tor'kren Farm =2979
3017 Frost Dagger Pass = 3017
3037 Ironstone Camp = 3037
3038 Weazel's Crater =3038
3039 Tahonda Ruins = 3039
3057 Field of Strife = 3057
3058 Icewing Cavern = 3058
3077 Valor's Rest =3077
3097 The Swarming Pillar = 3097
3098 Twilight Post = 3098
3099 Twilight Outpost = 3099
3100 Ravaged Twilight Camp = 3100
3117 Shalzaru's Lair =3117
3137 Talrendis Point = 3137
3138 Rethress Sanctum = 3138
3139 Moon Horror Den = 3139
3140 Scalebeard's Cave =3140
3157 Boulderslide Cavern = 3157
3177 Warsong Labor Camp = 3177
3197 Chillwind Camp = 3197
3217 The Maul = 3217
3237 The Maul = 3237
3257 Bones of Grakkarond = 3257
3277 Warsong Gulch = 3277
3297 Frostwolf Graveyard = 3297
3298 Frostwolf Pass = 3298
3299 Dun Baldar Pass = 3299
3300 Iceblood Graveyard = 3300
3301 Snowfall Graveyard = 3301
3302 Stonehearth Graveyard = 3302
3303 Stormpike Graveyard = 3303
3304 Icewing Bunker = 3304
3305 Stonehearth Bunker = 3305
3306 Wildpaw Ridge = 3306
3317 Revantusk Village = 3317
3318 Rock of Durotan = 3318
3319 Silverwing Grove = 3319
3320 Warsong Lumber Mill = 3320
3321 Silverwing Hold = 3321
3337 Wildpaw Cavern = 3337
3338 The Veiled Cleft = 3338
3357 Yojamba Isle = 3357
3358 Arathi Basin = 3358
3377 The Coil = 3377
3378 Altar of Hir'eek =3378
3379 Shadra'zaar =3379
3380 Hakkari Grounds = 3380
3381 Naze of Shirvallah = 3381
3382 Temple of Bethekk = 3382
3383 The Bloodfire Pit = 3383
3384 Altar of the Blood God = 3384
3397 Zanza's Rise =3397
3398 Edge of Madness = 3398
3417 Trollbane Hall = 3417
3418 Defiler's Den =3418
3419 Pagle's Pointe =3419
3420 Farm = 3420
3421 Blacksmith = 3421
3422 Lumber Mill = 3422
3423 Gold Mine = 3423
3424 Stables = 3424
3425 Cenarion Hold = 3425
3426 Staghelm Point = 3426
3427 Bronzebeard Encampment = 3427
3428 Ahn'Qiraj =3428
3429 Ruins of Ahn'Qiraj =3429
3430 Eversong Woods = 3430
3431 Sunstrider Isle = 3431
3432 Shrine of Dath'Remar =3432
3433 Ghostlands = 3433
3434 Scarab Terrace = 3434
3435 General's Terrace =3435
3436 The Reservoir = 3436
3437 The Hatchery = 3437
3438 The Comb = 3438
3439 Watchers' Terrace =3439
3440 Scarab Terrace = 3440
3441 General's Terrace =3441
3442 The Reservoir = 3442
3443 The Hatchery = 3443
3444 The Comb = 3444
3445 Watchers' Terrace =3445
3446 Twilight's Run =3446
3447 Ortell's Hideout =3447
3448 Scarab Terrace = 3448
3449 General's Terrace =3449
3450 The Reservoir = 3450
3451 The Hatchery = 3451
3452 The Comb = 3452
3453 Watchers' Terrace =3453
3454 Ruins of Ahn'Qiraj =3454
3455 The North Sea = 3455
3456 Naxxramas = 3456
3457 Karazhan = 3457
3460 Golden Strand = 3460
3461 Sunsail Anchorage = 3461
3462 Fairbreeze Village = 3462
3463 Magisters Gate = 3463
3464 Farstrider Retreat = 3464
3465 North Sanctum = 3465
3466 West Sanctum = 3466
3467 East Sanctum = 3467
3468 Saltheril's Haven =3468
3469 Thuron's Livery =3469
3470 Stillwhisper Pond = 3470
3471 The Living Wood = 3471
3472 Azurebreeze Coast = 3472
3473 Lake Elrendar = 3473
3474 The Scorched Grove = 3474
3475 Zeb'Watha =3475
3476 Tor'Watha =3476
3477 Karazhan = 3477
3478 Gates of Ahn'Qiraj =3478
3479 The Veiled Sea = 3479
3480 Duskwither Grounds = 3480
3481 Duskwither Spire = 3481
3482 The Dead Scar = 3482
3483 Hellfire Peninsula = 3483
3484 The Sunspire = 3484
3485 Falthrien Academy = 3485
3486 Ravenholdt Manor = 3486
3487 Silvermoon City = 3487
3488 Tranquillien = 3488
3489 Suncrown Village = 3489
3490 Goldenmist Village = 3490
3491 Windrunner Village = 3491
3492 Windrunner Spire = 3492
3493 Sanctum of the Sun = 3493
3494 Sanctum of the Moon = 3494
3495 Dawnstar Spire = 3495
3496 Farstrider Enclave = 3496
3497 An'daroth =3497
3498 An'telas =3498
3499 An'owyn =3499
3500 Deatholme = 3500
3501 Bleeding Ziggurat = 3501
3502 Howling Ziggurat = 3502
3503 Shalandis Isle = 3503
3504 Toryl Estate = 3504
3505 Underlight Mines = 3505
3506 Andilien Estate = 3506
3507 Hatchet Hills = 3507
3508 Amani Pass = 3508
3509 Sungraze Peak = 3509
3510 Amani Catacombs = 3510
3511 Tower of the Damned = 3511
3512 Zeb'Sora =3512
3513 Lake Elrendar = 3513
3514 The Dead Scar = 3514
3515 Elrendar River = 3515
3516 Zeb'Tela =3516
3517 Zeb'Nowa =3517
3518 Nagrand = 3518
3519 Terokkar Forest = 3519
3520 Shadowmoon Valley = 3520
3521 Zangarmarsh = 3521
3522 Blade's Edge Mountains =3522
3523 Netherstorm = 3523
3524 Azuremyst Isle = 3524
3525 Bloodmyst Isle = 3525
3526 Ammen Vale = 3526
3527 Crash Site = 3527
3528 Silverline Lake = 3528
3529 Nestlewood Thicket = 3529
3530 Shadow Ridge = 3530
3531 Skulking Row = 3531
3532 Dawning Lane = 3532
3533 Ruins of Silvermoon = 3533
3534 Feth's Way =3534
3535 Hellfire Citadel = 3535
3536 Thrallmar = 3536
3537 REUSE = 3537
3538 Honor Hold = 3538
3539 The Stair of Destiny = 3539
3540 Twisting Nether = 3540
3541 Forge Camp: Mageddon = 3541
3542 The Path of Glory = 3542
3543 The Great Fissure = 3543
3544 Plain of Shards = 3544
3545 Hellfire Citadel = 3545
3546 Expedition Armory = 3546
3547 Throne of Kil'jaeden =3547
3548 Forge Camp: Rage = 3548
3549 Invasion Point: Annihilator = 3549
3550 Borune Ruins = 3550
3551 Ruins of Sha'naar =3551
3552 Temple of Telhamat = 3552
3553 Pools of Aggonar = 3553
3554 Falcon Watch = 3554
3555 Mag'har Post =3555
3556 Den of Haal'esh =3556
3557 The Exodar = 3557
3558 Elrendar Falls = 3558
3559 Nestlewood Hills = 3559
3560 Ammen Fields = 3560
3561 The Sacred Grove = 3561
3562 Hellfire Ramparts = 3562
3563 Hellfire Citadel = 3563
3564 Emberglade = 3564
3565 Cenarion Refuge = 3565
3566 Moonwing Den = 3566
3567 Pod Cluster = 3567
3568 Pod Wreckage = 3568
3569 Tides' Hollow =3569
3570 Wrathscale Point = 3570
3571 Bristlelimb Village = 3571
3572 Stillpine Hold = 3572
3573 Odesyus' Landing =3573
3574 Valaar's Berth =3574
3575 Silting Shore = 3575
3576 Azure Watch = 3576
3577 Geezle's Camp =3577
3578 Menagerie Wreckage = 3578
3579 Traitor's Cove =3579
3580 Wildwind Peak = 3580
3581 Wildwind Path = 3581
3582 Zeth'Gor =3582
3583 Beryl Coast = 3583
3584 Blood Watch = 3584
3585 Bladewood = 3585
3586 The Vector Coil = 3586
3587 The Warp Piston = 3587
3588 The Cryo-Core = 3588
3589 The Crimson Reach = 3589
3590 Wrathscale Lair = 3590
3591 Ruins of Loreth'Aran =3591
3592 Nazzivian = 3592
3593 Axxarien = 3593
3594 Blacksilt Shore = 3594
3595 The Foul Pool = 3595
3596 The Hidden Reef = 3596
3597 Amberweb Pass = 3597
3598 Wyrmscar Island = 3598
3599 Talon Stand = 3599
3600 Bristlelimb Enclave = 3600
3601 Ragefeather Ridge = 3601
3602 Kessel's Crossing =3602
3603 Tel'athion's Camp = 3603
3604 The Bloodcursed Reef = 3604
3605 Hyjal Past = 3605
3606 Hyjal Summit = 3606
3607 Coilfang Reservoir = 3607
3608 Vindicator's Rest =3608
3610 Burning Blade Ruins = 3610
3611 Clan Watch = 3611
3612 Bloodcurse Isle = 3612
3613 Garadar = 3613
3614 Skysong Lake = 3614
3615 Throne of the Elements = 3615
3616 Laughing Skull Ruins = 3616
3617 Warmaul Hill = 3617
3618 Gruul's Lair =3618
3619 Auren Ridge = 3619
3620 Auren Falls = 3620
3621 Lake Sunspring = 3621
3622 Sunspring Post = 3622
3623 Aeris Landing = 3623
3624 Forge Camp: Fear = 3624
3625 Forge Camp: Hate = 3625
3626 Telaar = 3626
3627 Northwind Cleft = 3627
3628 Halaa = 3628
3629 Southwind Cleft = 3629
3630 Oshu'gun =3630
3631 Spirit Fields = 3631
3632 Shamanar = 3632
3633 Ancestral Grounds = 3633
3634 Windyreed Village = 3634
3636 Elemental Plateau = 3636
3637 Kil'sorrow Fortress =3637
3638 The Ring of Trials = 3638
3639 Silvermyst Isle = 3639
3640 Daggerfen Village = 3640
3641 Umbrafen Village = 3641
3642 Feralfen Village = 3642
3643 Bloodscale Enclave = 3643
3644 Telredor = 3644
3645 Zabra'jin =3645
3646 Quagg Ridge = 3646
3647 The Spawning Glen = 3647
3648 The Dead Mire = 3648
3649 Sporeggar = 3649
3650 Ango'rosh Grounds =3650
3651 Ango'rosh Stronghold =3651
3652 Funggor Cavern = 3652
3653 Serpent Lake = 3653
3654 The Drain = 3654
3655 Umbrafen Lake = 3655
3656 Marshlight Lake = 3656
3657 Portal Clearing = 3657
3658 Sporewind Lake = 3658
3659 The Lagoon = 3659
3660 Blades' Run =3660
3661 Blade Tooth Canyon = 3661
3662 Commons Hall = 3662
3663 Derelict Manor = 3663
3664 Huntress of the Sun = 3664
3665 Falconwing Square = 3665
3666 Halaani Basin = 3666
3667 Hewn Bog = 3667
3668 Boha'mu Ruins =3668
3669 The Stadium = 3669
3670 The Overlook = 3670
3671 Broken Hill = 3671
3672 Mag'hari Procession =3672
3673 Nesingwary Safari = 3673
3674 Cenarion Thicket = 3674
3675 Tuurem = 3675
3676 Veil Shienor = 3676
3677 Veil Skith = 3677
3678 Veil Shalas = 3678
3679 Skettis = 3679
3680 Blackwind Valley = 3680
3681 Firewing Point = 3681
3682 Grangol'var Village =3682
3683 Stonebreaker Hold = 3683
3684 Allerian Stronghold = 3684
3685 Bonechewer Ruins = 3685
3686 Veil Lithic = 3686
3687 Olembas = 3687
3688 Auchindoun = 3688
3689 Veil Reskk = 3689
3690 Blackwind Lake = 3690
3691 Lake Ere'Noru =3691
3692 Lake Jorune = 3692
3693 Skethyl Mountains = 3693
3694 Misty Ridge = 3694
3695 The Broken Hills = 3695
3696 The Barrier Hills = 3696
3697 The Bone Wastes = 3697
3698 Nagrand Arena = 3698
3699 Laughing Skull Courtyard = 3699
3700 The Ring of Blood = 3700
3701 Arena Floor = 3701
3702 Blade's Edge Arena =3702
3703 Shattrath City = 3703
3704 The Shepherd's Gate =3704
3705 Telaari Basin = 3705
3706 The Dark Portal = 3706
3707 Alliance Base = 3707
3708 Horde Encampment = 3708
3709 Night Elf Village = 3709
3710 Nordrassil = 3710
3711 Black Temple = 3711
3712 Area 52 = 3712
3713 The Blood Furnace = 3713
3714 The Shattered Halls = 3714
3715 The Steamvault = 3715
3716 The Underbog = 3716
3717 The Slave Pens = 3717
3718 Swamprat Post = 3718
3719 Bleeding Hollow Ruins = 3719
3720 Twin Spire Ruins = 3720
3721 The Crumbling Waste = 3721
3722 Manaforge Ara = 3722
3723 Arklon Ruins = 3723
3724 Cosmowrench = 3724
3725 Ruins of Enkaat = 3725
3726 Manaforge B'naar =3726
3727 The Scrap Field = 3727
3728 The Vortex Fields = 3728
3729 The Heap = 3729
3730 Manaforge Coruu = 3730
3731 The Tempest Rift = 3731
3732 Kirin'Var Village =3732
3733 The Violet Tower = 3733
3734 Manaforge Duro = 3734
3735 Voidwind Plateau = 3735
3736 Manaforge Ultris = 3736
3737 Celestial Ridge = 3737
3738 The Stormspire = 3738
3739 Forge Base: Oblivion = 3739
3740 Forge Base: Gehenna = 3740
3741 Ruins of Farahlon = 3741
3742 Socrethar's Seat =3742
3743 Legion Hold = 3743
3744 Shadowmoon Village = 3744
3745 Wildhammer Stronghold = 3745
3746 The Hand of Gul'dan =3746
3747 The Fel Pits = 3747
3748 The Deathforge = 3748
3749 Coilskar Cistern = 3749
3750 Coilskar Point = 3750
3751 Sunfire Point = 3751
3752 Illidari Point = 3752
3753 Ruins of Baa'ri =3753
3754 Altar of Sha'tar =3754
3755 The Stair of Doom = 3755
3756 Ruins of Karabor = 3756
3757 Ata'mal Terrace =3757
3758 Netherwing Fields = 3758
3759 Netherwing Ledge = 3759
3760 The Barrier Hills = 3760
3761 The High Path = 3761
3762 Windyreed Pass = 3762
3763 Zangar Ridge = 3763
3764 The Twilight Ridge = 3764
3765 Razorthorn Trail = 3765
3766 Orebor Harborage = 3766
3767 Blades' Run =3767
3768 Jagged Ridge = 3768
3769 Thunderlord Stronghold = 3769
3770 Blade Tooth Canyon = 3770
3771 The Living Grove = 3771
3772 Sylvanaar = 3772
3773 Bladespire Hold = 3773
3774 Gruul's Lair =3774
3775 Circle of Blood = 3775
3776 Bloodmaul Outpost = 3776
3777 Bloodmaul Camp = 3777
3778 Draenethyst Mine = 3778
3779 Trogma's Claim =3779
3780 Blackwing Coven = 3780
3781 Grishnath = 3781
3782 Veil Lashh = 3782
3783 Veil Vekh = 3783
3784 Forge Camp: Terror = 3784
3785 Forge Camp: Wrath = 3785
3786 Felstorm Point = 3786
3787 Forge Camp: Anger = 3787
3788 The Low Path = 3788
3789 Shadow Labyrinth = 3789
3790 Auchenai Crypts = 3790
3791 Sethekk Halls = 3791
3792 Mana - Tombs = 3792
3793 Felspark Ravine = 3793
3794 Valley of Bones = 3794
3795 Sha'naari Wastes =3795
3796 The Warp Fields = 3796
3797 Fallen Sky Ridge = 3797
3798 Haal'eshi Gorge =3798
3799 Stonewall Canyon = 3799
3800 Thornfang Hill = 3800
3801 Mag'har Grounds =3801
3802 Void Ridge = 3802
3803 The Abyssal Shelf = 3803
3804 The Legion Front = 3804
3805 Zul'Aman =3805
3806 Supply Caravan = 3806
3807 Reaver's Fall =3807
3808 Cenarion Post = 3808
3809 Southern Rampart = 3809
3810 Northern Rampart = 3810
3811 Gor'gaz Outpost =3811
3812 Spinebreaker Post = 3812
3813 The Path of Anguish = 3813
3814 East Supply Caravan = 3814
3815 Expedition Point = 3815
3816 Zeppelin Crash = 3816
3817 Testing = 3817
3818 Bloodscale Grounds = 3818
3819 Darkcrest Enclave = 3819
3820 Eye of the Storm = 3820
3821 Warden's Cage =3821
3822 Eclipse Point = 3822
3823 Isle of Tribulations = 3823
3824 Bloodmaul Ravine = 3824
3825 Dragons' End =3825
3826 Daggermaw Canyon = 3826
3827 Vekhaar Stand = 3827
3828 Ruuan Weald = 3828
3829 Veil Ruuan = 3829
3830 Raven's Wood =3830
3831 Death's Door =3831
3832 Vortex Pinnacle = 3832
3833 Razor Ridge = 3833
3834 Ridge of Madness = 3834
3835 Dustquill Ravine = 3835
3836 Magtheridon's Lair =3836
3837 Sunfury Hold = 3837
3838 Spinebreaker Mountains = 3838
3839 Abandoned Armory = 3839
3840 The Black Temple = 3840
3841 Darkcrest Shore = 3841
3842 Tempest Keep = 3842
3844 Mok'Nathal Village =3844
3845 Tempest Keep = 3845
3846 The Arcatraz = 3846
3847 The Botanica = 3847
3848 The Arcatraz = 3848
3849 The Mechanar = 3849
3850 Netherstone = 3850
3851 Midrealm Post = 3851
3852 Tuluman's Landing =3852
3854 Protectorate Watch Post = 3854
3855 Circle of Blood Arena = 3855
3856 Elrendar Crossing = 3856
3857 Ammen Ford = 3857
3858 Razorthorn Shelf = 3858
3859 Silmyr Lake = 3859
3860 Raastok Glade = 3860
3861 Thalassian Pass = 3861
3862 Churning Gulch = 3862
3863 Broken Wilds = 3863
3864 Bash'ir Landing =3864
3865 Crystal Spine = 3865
3866 Skald = 3866
3867 Bladed Gulch = 3867
3868 Gyro - Plank Bridge = 3868
3869 Mage Tower = 3869
3870 Blood Elf Tower = 3870
3871 Draenei Ruins = 3871
3872 Fel Reaver Ruins = 3872
3873 The Proving Grounds = 3873
3874 Eco - Dome Farfield = 3874
3875 Eco - Dome Skyperch = 3875
3876 Eco - Dome Sutheron = 3876
3877 Eco - Dome Midrealm = 3877
3878 Ethereum Staging Grounds = 3878
3879 Chapel Yard = 3879
3880 Access Shaft Zeon = 3880
3881 Trelleum Mine = 3881
3882 Invasion Point: Destroyer = 3882
3883 Camp of Boom = 3883
3884 Spinebreaker Pass = 3884
3885 Netherweb Ridge = 3885
3886 Derelict Caravan = 3886
3887 Refugee Caravan = 3887
3888 Shadow Tomb = 3888
3889 Veil Rhaze = 3889
3890 Tomb of Lights = 3890
3891 Carrion Hill = 3891
3892 Writhing Mound = 3892
3893 Ring of Observance = 3893
3894 Auchenai Grounds = 3894
3895 Cenarion Watchpost = 3895
3896 Aldor Rise = 3896
3897 Terrace of Light = 3897
3898 Scryer's Tier =3898
3899 Lower City = 3899
3900 Invasion Point: Overlord = 3900
3901 Allerian Post = 3901
3902 Stonebreaker Camp = 3902
3903 Boulder'mok =3903
3904 Cursed Hollow = 3904
3905 Coilfang Reservoir = 3905
3906 The Bloodwash = 3906
3907 Veridian Point = 3907
3908 Middenvale = 3908
3909 The Lost Fold = 3909
3910 Mystwood = 3910
3911 Tranquil Shore = 3911
3912 Goldenbough Pass = 3912
3913 Runestone Falithas = 3913
3914 Runestone Shan'dor =3914
3915 Fairbridge Strand = 3915
3916 Moongraze Woods = 3916
3917 Auchindoun = 3917
3918 Toshley's Station =3918
3919 Singing Ridge = 3919
3920 Shatter Point = 3920
3921 Arklonis Ridge = 3921
3922 Bladespire Outpost = 3922
3923 Gruul's Lair =3923
3924 Northmaul Tower = 3924
3925 Southmaul Tower = 3925
3926 Shattered Plains = 3926
3927 Oronok's Farm =3927
3928 The Altar of Damnation = 3928
3929 The Path of Conquest = 3929
3930 Eclipsion Fields = 3930
3931 Bladespire Grounds = 3931
3932 Sketh'lon Base Camp =3932
3933 Sketh'lon Wreckage =3933
3934 Town Square = 3934
3935 Wizard Row = 3935
3936 Deathforge Tower = 3936
3937 Slag Watch = 3937
3938 Sanctum of the Stars = 3938
3939 Dragonmaw Fortress = 3939
3940 The Fetid Pool = 3940
3942 Razaan's Landing =3942
3943 Invasion Point: Cataclysm = 3943
3944 The Altar of Shadows = 3944
3945 Netherwing Pass = 3945
3946 Wayne's Refuge =3946
3947 The Scalding Pools = 3947
3948 Brian and Pat Test = 3948
3949 Magma Fields = 3949
3950 Crimson Watch = 3950
3951 Evergrove = 3951
3952 Wyrmskull Bridge = 3952
3953 Scalewing Shelf = 3953
3954 Wyrmskull Tunnel = 3954
3955 Hellfire Basin = 3955
3956 The Shadow Stair = 3956
3957 Sha'tari Outpost =3957*/
            }

            return zoneName;
        }
    }
}
