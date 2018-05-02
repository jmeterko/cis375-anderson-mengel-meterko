using System;
using System.Drawing;
using System.Windows.Forms;

/* / ============= CIS 375 - Semester Project ============= \
 * Group Members:
 *      Alex Anderson
 *      Justin Mengel
 *      Jerad Meterko
 *      
 * Game Description:
 *      The player will be spawned into pre-determined rooms and have to escape to the next door. 
 *      Waves of 'zombie' enemies of different types and behavior will spawn into each room and try to attack the player.
 *      After the 3rd room, the final area contains a boss fight. If the player kills the boss, they win the game. Starting with 10 health,
 *      more can be picked up (1 spawn per room) for 5 health and 50 points. Switching guns with the (FN +) F1, f2 keys, the two weapons have different
 *      stats and behavior. One will shoot slower, but have better damage; while the other shoots faster with lower damage per hit. A cheat mode can be activated with F3,
 *      giving the player immunity to damage.
 *      
 * Game Controls:
 *      Arrow Keys - Directional Movement
 *      Space Bar  - Fire Weapon
 *      F1         - Default Weapon (Slower fire, higher damage)
 *      F2         - Alternate Weapon (Faster fire, lower damage)
 *      F3 (CHEAT) - Cheat Mode Toggle (Gives player immunity to damage)
 *      
 * Project Requirements:
 *      The following project requirements will be listed below along with descriptions of how they were
 *      fufilled within this project. Then the corresponding file location will be marked next to the description,
 *      along with a comment near the code that will match back to this list. 
 *      
 *      // Example requirement tag. These will be located above the code fufilling the labled requirement
 *      
 *      -->  
 *              //=== Requirement # 1 (Located in file: example.cs) ===//
 *      
 *      Requirements:
 *      (1) [Located in game3D.cs]: At least 1 collide function with a special effect not in the original version.
 *              Within each zombieClass, they will handle their collisions with the player seperately, dealing damage according to their stats. 
 *              There is also a treasure collision that will give the player 5 health and 50 points. 
 *      
 *      (2) [critterwall.cs]: Implement 1 moving wall (opening door or moving platform) Must be created in a class derived from cCritterWall.
 *              In the last room before the boss spawns, on the setRoom3(); method call, 4 walls start to slowly open revealing the final boss fight
 *              MovingWalls are created in setRoom3() and their movement is determined in the listener class
 *      
 *      (3) [game3D.cs]: Add 5 different model states for the project
 *              State.Run while player and critters are moving
 *              State.FallbackDie when critters shot
 *              State.ShotButStandingStill when firing
 *              State.ShotDown 
 *              State.ShotInShoulder when player is taking damage
 *              
 *      (4) [resource.cs]: Add 3 different models that currently are not in the acframework
 *              Models for slith, marine, tyrant, and insect added
 *      
 *      (5) [game3D.cs]: Add a permanent model other than the player
 *              Boss is added to the program as a permanent critter model using method spawnBoss();
 *      
 *      (6) [game3D.cs]: Add at least four distinct walls
 *              See methods setRoom1(), setRoom2(), and setRoom3(). 
 *              There is a lot of different walls. Average 5 different walls per room
 *              
 *      (7) [resource.cs]: Adding new textures for wall, floor, and ceiling.
 *              Metal, Mandala, and Ceiling textures added in the resource file. 
 *      
 *      (8) [game3D.cs]: (Enable a cheat to override loss)
 *              Pressing [Fn] + [F3] will make player invincible to damage. Boolean flag removes the check for collisions
 *   
 *      (9) [game3D.cs: Implement at least 3 distinct rooms with doorways between rooms (portal or teleportation device[s])
 *              Rooms (entered through teleportation doorways) COllision sets new room and door collision check 
 *              first default room cGame() constructor
 *              setRoom1()
 *              setRoom2()
 *              setRoom3()
 *              
 *      (10) [resource.cs]: Implement at least one key (on the keyboard) not included
 *              F1 - Sets default gun type with average damage
 *              F2 - Sets alternate gun type with higher fire and lower damage
 *              F3 - Sets the cheat mode so the player is immune to damamge
 *      
 *      ======================================================================
 *      
 *      (11) [game3D.cs]: Make at least 1 win to win and 1 way to lose
 *              Loss: Player health is reduced to 0 or less, game is over
 *              Win: Boss is killed and final score is displayed to user
 *              
 *      (12) [resource.cs]: Add in at least 3 sounds not included in the original version.
 *              See resource file for included sounds
 *              
 *      (13) [game3D.cs]: Implement one bullet class must be derived from one of the existing bullet classes.
 */

namespace ACFramework
{

    class cCritterDoor : cCritterWall
    {

        public cCritterDoor(cVector3 enda, cVector3 endb, float thickness, float height, cGame pownergame)
            : base(enda, endb, thickness, height, pownergame)
        {
        }

        public override bool collide(cCritter pcritter)
        {
            bool collided = base.collide(pcritter);
            if (collided && pcritter.IsKindOf("cCritter3DPlayer"))
            {
                ((cGame3D)Game).setdoorcollision();
                return true;
            }
            return false;
        }

        public override bool IsKindOf(string str)
        {
            return str == "cCritterDoor" || base.IsKindOf(str);
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritterDoor";
            }
        }
    }

    //==============Critters for the cGame3D: Player, Ball, Treasure ================ 

    class cCritter3DPlayer : cCritterArmedPlayer
    {
        public cCritter3DPlayer(cGame pownergame)
            : base(pownergame)
        {
            BulletClass = new cCritterNewBullet();
            Sprite = new cSpriteQuake(ModelsMD2.Marine);        //set player avatar to marine
            Sprite.SpriteAttitude = cMatrix3.scale(2, 0.8f, 0.4f);
            setRadius(cGame3D.PLAYERRADIUS);                    //Default cCritter.PLAYERRADIUS is 0.4.  
            setHealth(10);                                      //set health of player
            Mode = 'G';                                         //set default fire mode
            Cheat = false;                                      //set Cheat mode flag : off
            moveTo(_movebox.LoCorner.add(new cVector3(0.0f, 0.0f, 2.0f)));
            WrapFlag = cCritter.CLAMP; //Use CLAMP so you stop dead at edges.
            Armed = true; //Let's use bullets.
            MaxSpeed = cGame3D.MAXPLAYERSPEED;
            AbsorberFlag = true; //Keeps player from being buffeted about.
            ListenerAcceleration = 160.0f; //So Hopper can overcome gravity.  Only affects hop.

            Sprite.ModelState = State.Idle;

            // YHopper hop strength 12.0
            Listener = new cListenerQuakeScooterYHopper(0.2f, 12.0f);
            // the two arguments are walkspeed and hop strength -- JC

            addForce(new cForceGravity(50.0f)); /* Uses  gravity. Default strength is 25.0.
			Gravity	will affect player using cListenerHopper. */
            AttitudeToMotionLock = false; //It looks nicer is you don't turn the player with motion.
            Attitude = new cMatrix3(new cVector3(0.0f, 0.0f, -1.0f), new cVector3(-1.0f, 0.0f, 0.0f),
                new cVector3(0.0f, 1.0f, 0.0f), Position);


        }

        public override void update(ACView pactiveview, float dt)
        {
            base.update(pactiveview, dt); //Always call this first
        }

        public override bool collide(cCritter pcritter)
        {
            bool playerhigherthancritter = Position.Y - Radius > pcritter.Position.Y;
            /* If you are "higher" than the pcritter, as in jumping on it, you get a point
        and the critter dies.  If you are lower than it, you lose health and the
        critter also dies. To be higher, let's say your low point has to higher
        than the critter's center. We compute playerhigherthancritter before the collide,
        as collide can change the positions. */
            _baseAccessControl = 1;

            bool collided = base.collide(pcritter);
            _baseAccessControl = 0;
            if (!collided)
                return false;

            /* If you're here, you collided.  We'll treat all the guys the same -- the collision
         with a Treasure is different, but we let the Treasure contol that collision. */

            //=== Requirement # 8 (Enable cheat to override loss) ===//
            if (!Cheat)//only check collisions if cheat is not active
            {

                if (playerhigherthancritter)
                {
                    addScore(10);
                    pcritter.Sprite.ModelState = State.FallbackDie;
                    setForces(pcritter);
                }

                else if (pcritter.Sprite.ModelState == State.Run)
                {//awful way to check the type of sprite, but class variables were too big of a pain to check here
                 //if the sprite was a runner, deal 1 damage
                    Sprite.ModelState = State.ShotInShoulder;
                    if (pcritter.Sprite.ResourceID == 16003)
                    {
                        damage(1);
                        pcritter.Sprite.ModelState = State.FallbackDie;
                        setForces(pcritter);
                    }


                    else if (pcritter.IsKindOf("cCritterBoss"))
                    {
                        damage(1);

                        if (pcritter.Health < 1)
                        {
                            setIsAlive(false);
                        }
                    }

                    //if the sprite was a tank, deal 4
                    else if (pcritter.Sprite.ResourceID == 16002)
                    {
                        damage(3);
                        pcritter.Sprite.ModelState = State.FallbackDie;
                        setForces(pcritter);
                    }

                    //if the sprite was a walker, deal 2
                    else if (pcritter.Sprite.ResourceID == 16001)
                    {
                        damage(2);
                        pcritter.Sprite.ModelState = State.FallbackDie;
                        setForces(pcritter);
                    }

                    else
                    {
                        Sprite.ModelState = State.ShotDown;
                        damage(pcritter.getHitDamage());
                        Framework.snd.play(Sound.Crunch);
                        pcritter.Sprite.ModelState = State.FallbackDie;
                        setForces(pcritter);
                    }

                }

            }

            return true;

        }

        public override cCritterBullet shoot()
        {
            Framework.snd.play(Sound.Gunfire);
            Sprite.ModelState = State.ShotButStillStanding;
            return base.shoot();
        }

        public override bool IsKindOf(string str)
        {
            return str == "cCritter3DPlayer" || base.IsKindOf(str);
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DPlayer";
            }
        }

        public static char Mode { get; internal set; }
        public static bool Cheat { get; internal set; }
    }


    class cCritterNewBullet : cCritterBullet
    {
        public cCritterNewBullet() { }

        public override cCritterBullet Create()
        // has to be a Create function for every type of bullet -- JC
        {
            return new cCritterNewBullet();
        }

        public override void initialize(cCritterArmed pshooter)
        {
            base.initialize(pshooter);

            //if mode is set to game default - this weapon will do average damage, but has a slow delay between shots
            if (cCritter3DPlayer.Mode == 'G')
            {
                cCritterArmed.setShotWait(0.10f);    //sets the delay between bullets
                _hitstrength = 1;                    //sets the damage of bullet
                Sprite = new cSpriteSphere();        //sets the appearance of bullet (to sphere)
                Sprite.FillColor = Color.DarkKhaki;  //sets the color of bullet
                setRadius(0.2f);                     //sets the size of bullet
            }

            else//mode is F - alternate fire mode shoots faster than default bullets, but do less damage and bounce off walls
            {
                cCritterArmed.setShotWait(0.00f); //sets the delay between bullets
                _hitstrength = 2;                 //sets the damage of bullet
                _dieatedges = false;              //sets the bullet to bounce off walls
                Sprite = new cSpriteSphere();     //sets the appearance of bullet (to sphere)
                Sprite.FillColor = Color.Red;     //sets the color of bullet
                setRadius(0.1f);                  //sets the size of bullet            
            }
        }

        public override bool IsKindOf(string str)
        {
            return str == "cCritter3DPlayerBullet" || base.IsKindOf(str);
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DPlayerBullet";
            }
        }

        public override bool collide(cCritter pcritter)
        {   //if the bullet hits a critter
            if (isTarget(pcritter) && touch(pcritter))
            {
                delete_me();//destroys bullet on critter collision (so shots don't pierce targets)

                //if the target critter's health is still good
                if(pcritter.Health > 0)
                {
                    //if the default fire mode is on
                    if (cCritter3DPlayer.Mode == 'G')
                    {
                        pcritter.loseHealth(-2);//make the critter take 2 damage
                    }

                    //otherwise it is on rapid fire
                    else
                    {
                        pcritter.loseHealth(-1);//make the critter take 1 damage

                    }
                }

                //if the target critter's health is not so good (0 or less)
                else
                {
                    //set animation to dying, clear all forces, show critter slumped on ground then kill it
                    pcritter.Sprite.ModelState = State.FallbackDie;
                    pcritter.clearForcelist();
                    pcritter.addForce(new cForceDrag(50.0f));
                    pcritter.addForce(new cForceGravity(25.0f, new cVector3(0, 0, 0)));
                    setForces(pcritter);
                    //add score for killing a Critter
                    Player.addScore(1);
                }

                return true;
            }
            return false;
        }
    }

    //=== Requirement # 5 (Make a permanent model class) ===//
    class cCritterBoss : cCritter
    {
        public cCritterBoss(cGame pownergame)
            : base(pownergame)
        {
            addForce(new cForceGravity(25.0f, new cVector3(0.0f, -1, 0.00f)));
            addForce(new cForceDrag(0.5f));  // default friction strength 0.5 
            Density = 2.0f;
            MaxSpeed = 10.0f;
            setHealth(10);
            if (pownergame != null) //Just to be safe.
                Sprite = new cSpriteQuake(ModelsMD2.Tyrant);

            addForce(new cForceObjectSeek(Player, 0.5f));

            

            // example of setting a specific model
            // setSprite(new cSpriteQuake(ModelsMD2.Knight));

            if (Sprite.IsKindOf("cSpriteQuake")) //Don't let the figurines tumble.  
            {
                AttitudeToMotionLock = false;
                Attitude = new cMatrix3(new cVector3(0.0f, 0.0f, 1.0f),
                    new cVector3(1.0f, 0.0f, 0.0f),
                    new cVector3(0.0f, 1.0f, 0.0f), Position);
                /* Orient them so they are facing towards positive Z with heads towards Y. */
            }
            Bounciness = 0.0f; //Not 1.0 means it loses a bit of energy with each bounce.
            setRadius(6.0f);   //set size of generated critter sprites
            MinTwitchThresholdSpeed = 4.0f; //Means sprite doesn't switch direction unless it's moving fast 
            randomizePosition(new cRealBox3(new cVector3(_movebox.Lox, _movebox.Loy, _movebox.Loz + 4.0f),
                new cVector3(_movebox.Hix, _movebox.Loy, _movebox.Midz - 1.0f)));
            /* I put them ahead of the player  */
            randomizeVelocity(0.0f, 30.0f, false);


            if (pownergame != null) //Then we know we added this to a game so pplayer() is valid 
                addForce(new cForceObjectSeek(Player, 0.5f));

            int begf = Framework.randomOb.random(0, 171);
            int endf = Framework.randomOb.random(0, 171);

            if (begf > endf)
            {
                int temp = begf;
                begf = endf;
                endf = temp;
            }

            Sprite.setstate(State.Run, begf, endf, StateType.Repeat);

            _wrapflag = cCritter.BOUNCE;

        }


        public override void update(ACView pactiveview, float dt)
        {
            base.update(pactiveview, dt); //Always call this first
        }

        // do a delete_me if you hit the left end 

        public override void die()
        {
            base.die();
        }

        public override bool IsKindOf(string str)
        {
            return str == "cCritterBoss" || base.IsKindOf(str);
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritterBoss";
            }
        }
    }


    class cCritterZombie : cCritter
    {
        public cCritterZombie(cGame pownergame, int zombieType)
            : base(pownergame)
        {
            addForce(new cForceGravity(25.0f, new cVector3(0.0f, -1, 0.00f)));
            addForce(new cForceDrag(0.5f));  // default friction strength 0.5 
            Density = 2.0f;
            MaxSpeed = 10.0f;
            if (pownergame != null) //Just to be safe.

            addForce(new cForceObjectSeek(Player, 0.5f));

            if (zombieType < 15)
            {
                Sprite = new cSpriteQuake(ModelsMD2.Slith);
            }

            else if (zombieType < 20)
            {
                Sprite = new cSpriteQuake(ModelsMD2.Tyrant);
            }

            else
            {
                Sprite = new cSpriteQuake(ModelsMD2.Runner);
            }

            // example of setting a specific model
            // setSprite(new cSpriteQuake(ModelsMD2.Knight));

            if (Sprite.IsKindOf("cSpriteQuake")) //Don't let the figurines tumble.  
            {
                AttitudeToMotionLock = false;
                Attitude = new cMatrix3(new cVector3(0.0f, 0.0f, 1.0f),
                    new cVector3(1.0f, 0.0f, 0.0f),
                    new cVector3(0.0f, 1.0f, 0.0f), Position);
                /* Orient them so they are facing towards positive Z with heads towards Y. */
            }
            Bounciness = 0.0f; //Not 1.0 means it loses a bit of energy with each bounce.
            setRadius(1.0f);   //set size of generated critter sprites
            MinTwitchThresholdSpeed = 4.0f; //Means sprite doesn't switch direction unless it's moving fast 
            randomizePosition(new cRealBox3(new cVector3(_movebox.Lox, _movebox.Loy, _movebox.Loz + 4.0f),
                new cVector3(_movebox.Hix, _movebox.Loy, _movebox.Midz - 1.0f)));
            /* I put them ahead of the player  */
            randomizeVelocity(0.0f, 30.0f, false);


            if (pownergame != null) //Then we know we added this to a game so pplayer() is valid 
                addForce(new cForceObjectSeek(Player, 0.5f));

            int begf = Framework.randomOb.random(0, 171);
            int endf = Framework.randomOb.random(0, 171);

            if (begf > endf)
            {
                int temp = begf;
                begf = endf;
                endf = temp;
            }

            Sprite.setstate(State.Run, begf, endf, StateType.Repeat);

            _wrapflag = cCritter.BOUNCE;

        }


        public override void update(ACView pactiveview, float dt)
        {
            base.update(pactiveview, dt); //Always call this first
        }

        // do a delete_me if you hit the left end 

        public override void die()
        {
            base.die();
        }

        public override bool IsKindOf(string str)
        {
            return str == "cCritterZombie" || base.IsKindOf(str);
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritterZombie";
            }
        }
    }

    class zombieWalker : cCritterZombie
    {
        public zombieWalker(cGame pownergame, int zombietype)
            : base(pownergame, zombietype)
        {
                setHitDamage(2);
                setHealth(2);
        }

        public override bool collide(cCritter pcritter)
        {
            damage(pcritter.getHitDamage());
            //Framework.snd.play(Sound.Crunch);

            return true;
        }
    }

    class zombieTank : cCritterZombie
    {
        public zombieTank(cGame pownergame, int zombieType)
            : base(pownergame, zombieType)
        {
            setHitDamage(4);
            setHealth(4);
            _maxspeed = 1.5f;
        }

        public override bool collide(cCritter pcritter)
        {
            damage(pcritter.getHitDamage());
            //Framework.snd.play(Sound.Crunch);

            return true;
        }
    }

    class zombieRunner : cCritterZombie
    {
        public zombieRunner(cGame pownergame, int zombietype)
            : base(pownergame, zombietype)
        {
            setHitDamage(1);
            setHealth(1);
            _maxspeed = 5;
            Sprite = new cSpriteQuake(ModelsMD2.Runner);
        }


        public override bool collide(cCritter pcritter)
        {
            damage(pcritter.getHitDamage());
            //Framework.snd.play(Sound.Crunch);
            return true;

        }
    }


    class cCritterTreasure : cCritter
    {

        Random randPos = new Random();

        public cCritterTreasure(cGame pownergame) :
        base(pownergame)
        {
            //set a polygon to a red diamond shape
            cPolygon ppoly = new cPolygon();
            ppoly.Filled = true;
            ppoly.LineWidthWeight = 1.0f;
            ppoly.setStarPolygon(2,1);
            ppoly.FillColor = Color.Red;

            //set the treasure to the created shape
            Sprite = ppoly;

            //set size and fix it in place so enemies don't push it around
            setRadius(cGame3D.TREASURERADIUS);
            FixedFlag = true;
            

            //make a random number to place treasure in room
            moveTo(new cVector3(randPos.Next(-32,32), -15, randPos.Next(-32, 32)));
        }

        //=== Requirement # 1 (1 New collision function) ===//
        public override bool collide(cCritter pcritter)
        {
            if (distanceTo(pcritter) + pcritter.Radius < Radius + 1) //if player gets within radius+1 of pickup (close but not inside of it)
            {
                //play sound for pickup, add score, add health, and make the treasure go away.
                Framework.snd.play(Sound.Healing);
                pcritter.addScore(50);
                pcritter.addHealth(5);
                _collecteditem = true;  //set flag to determine when to spawn new item

                //this makes the pickup respawn in a new random location on pickup
                //moveTo(new cVector3(randPos.Next(-32, 32), -15, randPos.Next(-32, 32)));

                //this removes the pickup from the current room, so it doesn't respawn
                delete_me();            //delete the pickup, now that it has been used
                return true;
            }

            else
            {
                return false;
            }
            //return false;
        }

        //Checks if pcritter inside.

        public override int collidesWith(cCritter pothercritter)
        {
            if (pothercritter.IsKindOf("cCritter3DPlayer"))
                return cCollider.COLLIDEASCALLER;
            else
                return cCollider.DONTCOLLIDE;
        }

        /* Only collide
			with cCritter3DPlayer. */

        public override bool IsKindOf(string str)
        {
            return str == "cCritterTreasure" || base.IsKindOf(str);
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritterTreasure";
            }
        }
    }

    //======================cGame3D========================== 
    class cGame3D : cGame
    {
        public static readonly float TREASURERADIUS = 1.0f;
        public static readonly float WALLTHICKNESS = 0.5f;
        public static readonly float PLAYERRADIUS = 0.5f; //sets the player size
        public static readonly float MAXPLAYERSPEED = 10.0f; //sets the player speed
        private cCritterTreasure _ptreasure;
        private bool doorcollision;
        private bool wentThrough = false;
        private float startNewRoom;
        cCritter boss;

        public cGame3D()
        {
            doorcollision = false;
            _menuflags &= ~cGame.MENU_BOUNCEWRAP;
            _menuflags |= cGame.MENU_HOPPER; //Turn on hopper listener option.
            _spritetype = cGame.ST_MESHSKIN;
            setBorder(64.0f, 16.0f, 64.0f); // size of the world

            cRealBox3 skeleton = new cRealBox3();
            skeleton.copy(_border);
            setSkyBox(skeleton);
            /* In this world the coordinates are screwed up to match the screwed up
            listener that I use.  I should fix the listener and the coords.
            Meanwhile...
            I am flying into the screen from HIZ towards LOZ, and
            LOX below and HIX above and
            LOY on the right and HIY on the left. */
            SkyBox.setSideTexture(cRealBox3.HIZ, BitmapRes.Wall1); //Make the near HIZ transparent 
            SkyBox.setSideTexture(cRealBox3.LOZ, BitmapRes.Wall1); //Far wall 
            SkyBox.setSideTexture(cRealBox3.LOX, BitmapRes.Wall1); //left wall 
            SkyBox.setSideTexture(cRealBox3.HIX, BitmapRes.Wall1); //right wall 
            SkyBox.setSideTexture(cRealBox3.LOY, BitmapRes.Metal); //floor 
            SkyBox.setSideTexture(cRealBox3.HIY, BitmapRes.Ceiling); //ceiling 

            WrapFlag = cCritter.BOUNCE;

            //set variables to control amount of zombie critters to spawn
            _seedcount = 5;
            _walkerscount = 3;
            _runnerscount = 1;
            _tankscount = 1;
            _currentroom = 0;

            setPlayer(new cCritter3DPlayer(this));
            _ptreasure = new cCritterTreasure(this);


            //create a critter door and set its size, location, and graphics
            cCritterDoor pdwall = new cCritterDoor(
                new cVector3(_border.Lox + 25, _border.Loy, _border.Midz - 32),
                new cVector3(_border.Lox + 25, _border.Midy - 3, _border.Midz - 32),
                5f, 0.1f, this);

            cSpriteTextureBox pspritedoor = new cSpriteTextureBox(pdwall.Skeleton, BitmapRes.Mandala);
            pdwall.Sprite = pspritedoor;

            /* In this world the x and y go left and up respectively, while z comes out of the screen.
		A wall views its "thickness" as in the y direction, which is up here, and its
		"height" as in the z direction, which is into the screen. */
            //First draw a wall with dy height resting on the bottom of the world.
            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 0.5f * _border.YSize;
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;

            //make a bunch of walls to create room layout
            cCritterWall pwall = new cCritterWall(
                new cVector3(_border.Midx + 0.0f, ycenter, zpos),
                new cVector3(_border.Hix, ycenter, zpos),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall2 = new cCritterWall(
                new cVector3(_border.Midx - 2.0f, ycenter, zpos + 10.0f),
                new cVector3(_border.Hix - 32.0f, ycenter, zpos),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall3 = new cCritterWall(
                new cVector3(_border.Midx - 90.0f, ycenter, zpos + 15.0f),
                new cVector3(_border.Hix - 45.0f, ycenter, zpos + 1.0f),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall4 = new cCritterWall(
                new cVector3(_border.Midx - 2.0f, ycenter, zpos + 10.0f),
                new cVector3(_border.Hix - 2.0f, ycenter, zpos),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall5 = new cCritterWall(
                new cVector3(_border.Midx - 20, ycenter, zpos - 5.0f),
                new cVector3(_border.Hix - 20, ycenter, zpos - 35.0f),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            //set texture of the walls
            cSpriteTextureBox pspritebox1 = new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox2 = new cSpriteTextureBox(pwall2.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox3 = new cSpriteTextureBox(pwall3.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox4 = new cSpriteTextureBox(pwall4.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox5 = new cSpriteTextureBox(pwall5.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 

            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */
            pwall.Sprite = pspritebox1;
            pwall2.Sprite = pspritebox2;
            pwall3.Sprite = pspritebox3;
            pwall4.Sprite = pspritebox4;
            pwall5.Sprite = pspritebox5;
        }

        //setRoom1 creates a new room when the player runs through the previous door
        public void setRoom1()
        {
            //remove critters and wall
            Biota.purgeCritters("cCritterWall");
            Biota.purgeCritters("cCritterZombie");
            Biota.purgeCritters("zombieWalker");
            Biota.purgeCritters("zombieTank");
            Biota.purgeCritters("zombieRunner");
            Biota.purgeCritters("cCritterTreasure");

            _zombiecount = 0;//reset count so seedCritters can be restarted

            setBorder(64.0f, 16.0f, 64.0f); // size of the world

            //create new room 'shell'
            cRealBox3 skeleton = new cRealBox3();
            skeleton.copy(_border);
            setSkyBox(skeleton);

            //set textures and graphics
            SkyBox.setSideTexture(cRealBox3.HIZ, BitmapRes.Wall1); //Make the near HIZ transparent 
            SkyBox.setSideTexture(cRealBox3.LOZ, BitmapRes.Wall1); //Far wall 
            SkyBox.setSideTexture(cRealBox3.LOX, BitmapRes.Wall1); //left wall 
            SkyBox.setSideTexture(cRealBox3.HIX, BitmapRes.Wall1); //right wall 
            SkyBox.setSideTexture(cRealBox3.LOY, BitmapRes.Metal); //floor 
            SkyBox.setSideTexture(cRealBox3.HIY, BitmapRes.Ceiling); //ceiling 

            //set number of critters to be created. Adjust numbers for increasing difficulty between rooms
            //set variables to control amount of zombie critters to spawn
            _seedcount = 10;
            _walkerscount = 4;
            _runnerscount = 4;
            _tankscount = 2;
            _currentroom = 1;

            WrapFlag = cCritter.BOUNCE;
            _ptreasure = new cCritterTreasure(this);

            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 0.5f * _border.YSize;
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;

            //create a critter door and set its size, location, and graphics
            cCritterDoor pdwall = new cCritterDoor(
                     new cVector3(_border.Lox + 45, _border.Loy, _border.Midz - 32),
                     new cVector3(_border.Lox + 45, _border.Midy - 3, _border.Midz - 32),
                     5f, 0.1f, this);

            cSpriteTextureBox pspritedoor = new cSpriteTextureBox(pdwall.Skeleton, BitmapRes.Mandala);
            pdwall.Sprite = pspritedoor;

            //make a bunch of walls to create room layout
            cCritterWall pwall = new cCritterWall(
                new cVector3(_border.Midx + 5, ycenter, zpos),
                new cVector3(_border.Hix + 5, ycenter, zpos),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall3 = new cCritterWall(
                new cVector3(_border.Midx - 35, ycenter, zpos - 10),
                new cVector3(_border.Hix - 35, ycenter, zpos - 10),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall4 = new cCritterWall(
                new cVector3(_border.Midx - 10, ycenter, zpos - 45),
                new cVector3(_border.Hix - 10, ycenter, zpos - 5),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);
            
            cCritterWall pwall7 = new cCritterWall(
                new cVector3(_border.Midx - 20, ycenter, zpos - 25),
                new cVector3(_border.Hix - 20, ycenter, zpos + 25),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall8 = new cCritterWall(
                new cVector3(_border.Midx - 25, ycenter - 10, zpos),
                new cVector3(_border.Hix + 10, ycenter + 22, zpos),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness + 4, //height argument for this wall's dz  goes into the screen 
                this);

            //set texture of the walls
            cSpriteTextureBox pspritebox = new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox3 = new cSpriteTextureBox(pwall3.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox4 = new cSpriteTextureBox(pwall4.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox7 = new cSpriteTextureBox(pwall7.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox8 = new cSpriteTextureBox(pwall8.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 

            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */
            pwall.Sprite = pspritebox;
            pwall3.Sprite = pspritebox3;
            pwall4.Sprite = pspritebox4;
            pwall7.Sprite = pspritebox7;
            pwall8.Sprite = pspritebox8;

            //set collision flag and reset age of new room
            wentThrough = true;
            startNewRoom = Age;

            seedCritters();//make new critters for room
                           
            //move player to new position in next room
            Player.moveTo(new cVector3(-33.0f, 0.0f, -25.0f));
        }

        //setRoom2 creates a new room when the player runs through the previous door
        public void setRoom2()
        {
            //remove critters and wall
            Biota.purgeCritters("cCritterWall");
            Biota.purgeCritters("cCritterZombie");
            Biota.purgeCritters("zombieWalker");
            Biota.purgeCritters("zombieTank");
            Biota.purgeCritters("zombieRunner");
            Biota.purgeCritters("cCritterTreasure");

            _zombiecount = 0;//reset count so seedCritters can be restarted

            setBorder(64.0f, 16.0f, 64.0f); // size of the world

            //create new room 'shell'
            cRealBox3 skeleton = new cRealBox3();
            skeleton.copy(_border);
            setSkyBox(skeleton);

            //set textures and graphics
            SkyBox.setSideTexture(cRealBox3.HIZ, BitmapRes.Wall1); //Make the near HIZ transparent 
            SkyBox.setSideTexture(cRealBox3.LOZ, BitmapRes.Wall1); //Far wall 
            SkyBox.setSideTexture(cRealBox3.LOX, BitmapRes.Wall1); //left wall 
            SkyBox.setSideTexture(cRealBox3.HIX, BitmapRes.Wall1); //right wall 
            SkyBox.setSideTexture(cRealBox3.LOY, BitmapRes.Metal); //floor 
            SkyBox.setSideTexture(cRealBox3.HIY, BitmapRes.Ceiling); //ceiling 

            //set number of critters to be created. Adjust numbers for increasing difficulty between rooms
            //set variables to control amount of zombie critters to spawn
            _seedcount = 12;
            _walkerscount = 0;
            _runnerscount = 10;
            _tankscount = 2;
            _currentroom = 2;

            WrapFlag = cCritter.BOUNCE;
            _ptreasure = new cCritterTreasure(this);

            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 0.5f * _border.YSize;
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;

            //create a critter door and set its size, location, and graphics
            cCritterDoor pdwall = new cCritterDoor(
                new cVector3(_border.Lox + 15, _border.Loy, _border.Midz - 32),
                new cVector3(_border.Lox + 15, _border.Midy - 3, _border.Midz - 32),
                5f, 0.1f, this);

            cSpriteTextureBox pspritedoor = new cSpriteTextureBox(pdwall.Skeleton, BitmapRes.Mandala);
            pdwall.Sprite = pspritedoor;


            cCritterWall pwall = new cCritterWall(
                new cVector3(_border.Midx - 35, ycenter - 10, zpos + 10),
                new cVector3(_border.Hix - 30, ycenter + 22, zpos + 20),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness + 4, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall2 = new cCritterWall(
                new cVector3(_border.Midx + 5, ycenter + 4, zpos),
                new cVector3(_border.Hix + 0, ycenter - 12, zpos),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness + 4, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall3 = new cCritterWall(
                new cVector3(_border.Midx - 5, ycenter, zpos - 10),
                new cVector3(_border.Hix - 5, ycenter + 20, zpos - 10),
                height, //thickness param for wall's dy which goes perpendicular to the 
                wallthickness + 4, //height argument for this wall's dz  goes into the screen 
                this);

            cCritterWall pwall4 = new cCritterWall(
               new cVector3(_border.Midx - 20 , ycenter + 20, zpos - 5f),
               new cVector3(_border.Hix - 20, ycenter - 30, zpos - 5),
               height, //thickness param for wall's dy which goes perpendicular to the 
               wallthickness + 4, //height argument for this wall's dz  goes into the screen 
               this);

            cCritterWall pwall5 = new cCritterWall(
               new cVector3(_border.Midx - 25, ycenter + 20, zpos + 15f),
               new cVector3(_border.Hix - 25, ycenter - 30, zpos + 15),
               height, //thickness param for wall's dy which goes perpendicular to the 
               wallthickness + 4, //height argument for this wall's dz  goes into the screen 
               this);

            //wall texture
            cSpriteTextureBox pspritebox = new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox2 = new cSpriteTextureBox(pwall2.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox3 = new cSpriteTextureBox(pwall3.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox4 = new cSpriteTextureBox(pwall4.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 
            cSpriteTextureBox pspritebox5 = new cSpriteTextureBox(pwall5.Skeleton, BitmapRes.Wall3, 16); //Sets all sides 

            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */
            pwall.Sprite = pspritebox;
            pwall2.Sprite = pspritebox2;
            pwall3.Sprite = pspritebox3;
            pwall4.Sprite = pspritebox4;
            pwall5.Sprite = pspritebox4;

            //move player to new position in next room
            Player.moveTo(new cVector3(0.0f, -10.0f, 32.0f));

            //set collision flag and reset age of new room
            wentThrough = true;
            startNewRoom = Age;

            seedCritters();//make new critters for room
        }

        //creates a new room when the player runs through the previous door
        public void setRoom3()
        {
            //remove critters and wall
            Biota.purgeCritters("cCritterWall");
            Biota.purgeCritters("cCritterZombie");
            Biota.purgeCritters("zombieWalker");
            Biota.purgeCritters("zombieTank");
            Biota.purgeCritters("zombieRunner");
            Biota.purgeCritters("cCritterTreasure");

            _zombiecount = 0;//reset count so seedCritters can be restarted

            setBorder(64.0f, 16.0f, 64.0f); // size of the world

            //create new room 'shell'
            cRealBox3 skeleton = new cRealBox3();
            skeleton.copy(_border);
            setSkyBox(skeleton);

            //set textures and graphics
            SkyBox.setSideTexture(cRealBox3.HIZ, BitmapRes.Wall1); //Make the near HIZ transparent 
            SkyBox.setSideTexture(cRealBox3.LOZ, BitmapRes.Wall1); //Far wall 
            SkyBox.setSideTexture(cRealBox3.LOX, BitmapRes.Wall1); //left wall 
            SkyBox.setSideTexture(cRealBox3.HIX, BitmapRes.Wall1); //right wall 
            SkyBox.setSideTexture(cRealBox3.LOY, BitmapRes.Metal); //floor 
            SkyBox.setSideTexture(cRealBox3.HIY, BitmapRes.Ceiling); //ceiling 

            //set number of critters to be created. Adjust numbers for increasing difficulty between rooms
            //set variables to control amount of zombie critters to spawn
            _seedcount = 7;
            _walkerscount = 0;
            _runnerscount = 4;
            _tankscount = 3;
            _currentroom = 3;

            WrapFlag = cCritter.BOUNCE;
            _ptreasure = new cCritterTreasure(this);


            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 0.5f * _border.YSize;
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;

            movingWall1 = new cCritterMovingWall(
                           new cVector3(-_border.Midx, -ycenter, -zpos),
                            new cVector3(-_border.Hix, -ycenter, -zpos),
                            height, //thickness param for wall's dy which goes perpendicular to the 
                            wallthickness, //height argument for this wall's dz  goes into the screen 
                            this);

            movingWall2 = new cCritterMovingWall(
                          new cVector3(_border.Midx, ycenter, zpos),
                           new cVector3(_border.Hix, ycenter, zpos),
                           height, //thickness param for wall's dy which goes perpendicular to the 
                           wallthickness, //height argument for this wall's dz  goes into the screen 
                           this);

            movingWall3 = new cCritterMovingWall(
                          new cVector3(-_border.Midx, -ycenter, -zpos),
                           new cVector3(_border.Hix, -ycenter, zpos),
                           height, //thickness param for wall's dy which goes perpendicular to the 
                           wallthickness, //height argument for this wall's dz  goes into the screen 
                           this);

            movingWall4 = new cCritterMovingWall(
                          new cVector3(_border.Midx, ycenter, -zpos),
                           new cVector3(-_border.Hix, ycenter, -zpos),
                           height, //thickness param for wall's dy which goes perpendicular to the 
                           wallthickness, //height argument for this wall's dz  goes into the screen 
                           this);

     
            cSpriteTextureBox movingWallSpriteBox = new cSpriteTextureBox(movingWall1.Skeleton, BitmapRes.Metal, 1); //Sets all sides 
            cSpriteTextureBox movingWallSpriteBox2 = new cSpriteTextureBox(movingWall2.Skeleton, BitmapRes.Metal, 1); //Sets all sides 
            cSpriteTextureBox movingWallSpriteBox3 = new cSpriteTextureBox(movingWall3.Skeleton, BitmapRes.Metal, 1); //Sets all sides 
            cSpriteTextureBox movingWallSpriteBox4 = new cSpriteTextureBox(movingWall4.Skeleton, BitmapRes.Metal, 1); //Sets all sides 
            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */

            movingWall1.Sprite = movingWallSpriteBox;
            movingWall2.Sprite = movingWallSpriteBox2;
            movingWall3.Sprite = movingWallSpriteBox3;
            movingWall4.Sprite = movingWallSpriteBox4;
            moveWalls = true;

            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */

            //move player to new position in next room
            Player.moveTo(new cVector3(0.0f, -10.0f, 32.0f));

            //set collision flag and reset age of new room
            wentThrough = true;
            startNewRoom = Age;

            spawnBoss();
            seedCritters();//make new critters for room
            

            
        }

        public override void seedCritters()
        {
            int currentRunners = 0;
            int currentTanks = 0;
            int currentWalkers = 0;

            Biota.purgeCritters("cCritterBullet");
            Biota.purgeCritters("cCritterZombie");
            for (int i = 0; i < _seedcount; i++)
            {

                /* Logic to Zombie Type Spawns :
                 * The counts for types of zombies are set in cGame and setRooms
                 * The local currentCounts of seededCritters will determine how many of each type to spawn
                 * The below conditionals will check what the number is, then set the zombies' health, speed, and damage hit strength
                 * Set the sprite to the specific type, and make sure to set the zombie to the correct class
                 */
                if (currentWalkers < _walkerscount)
                {
                    _zombietype = 1;//set type for zombie parameter
                    cCritterZombie spawn = new cCritterZombie(this, _zombietype);//create new zombie spawn
                    spawn = new zombieWalker(this, _zombietype);//set spawn to correct type
                    currentWalkers++;//increase the local count
                }
                
                else if (currentTanks < _tankscount)
                {
                    _zombietype = 19;
                    cCritterZombie spawn = new cCritterZombie(this, _zombietype);
                    spawn = new zombieTank(this, _zombietype);
                    currentTanks++;
                }

                else if (currentRunners < _runnerscount)
                {
                    _zombietype = 28;
                    cCritterZombie spawn = new cCritterZombie(this, _zombietype);
                    spawn = new zombieRunner(this, _zombietype);
                    currentRunners++;
                }
                    _zombiecount++;
            }

            Player.moveTo(new cVector3(0.0f, Border.Loy, Border.Hiz - 3.0f));
            /* We start at hiz and move towards	loz */
        }

        public void spawnBoss()
        {
            boss =  new cCritterBoss(this);
            Player.moveTo(new cVector3(0.0f, Border.Loy, Border.Hiz - 3.0f));
        }


        public void setdoorcollision() { doorcollision = true; }

        public override ACView View
        {
            set
            {
                base.View = value; //You MUST call the base class method here.
                value.setUseBackground(ACView.FULL_BACKGROUND); /* The background type can be
			    ACView.NO_BACKGROUND, ACView.SIMPLIFIED_BACKGROUND, or 
			    ACView.FULL_BACKGROUND, which often means: nothing, lines, or
			    planes&bitmaps, depending on how the skybox is defined. */
                value.pviewpointcritter().Listener = new cListenerViewerRide();
            }
        }


        public override cCritterViewer Viewpoint
        {
            set
            {
                if (value.Listener.RuntimeClass == "cListenerViewerRide")
                {
                    value.setViewpoint(new cVector3(0.0f, 0.3f, -1.0f), _border.Center);
                    //Always make some setViewpoint call simply to put in a default zoom.
                    value.zoom(0.05f); //Wideangle 
                    cListenerViewerRide prider = (cListenerViewerRide)(value.Listener);
                    prider.Offset = (new cVector3(-1.5f, 0.0f, 11.0f)); /* This offset is in the coordinate
				    system of the player, where the negative X axis is the negative of the
				    player's tangent direction, which means stand right behind the player. */
                }
                else //Not riding the player.
                {
                    value.zoom(1.0f);
                    /* The two args to setViewpoint are (directiontoviewer, lookatpoint).
				    Note that directiontoviewer points FROM the origin TOWARDS the viewer. */
                    value.setViewpoint(new cVector3(0.0f, 0.3f, 1.0f), _border.Center);
                }
            }
        }

        /* Move over to be above the
			lower left corner where the player is.  In 3D, use a low viewpoint low looking up. */

        public override void adjustGameParameters()
        {
            
            if(boss != null && boss.Health <= 0)
            {
                _gameover = true;

                Player.addScore(_scorecorrection); // So user can reach _maxscore  
                //Framework.snd.play(Sound.Hallelujah);
                return;
            }

            if(cCritter.isAlive == false)
            {
                _gameover = true;

                Player.addScore(_scorecorrection); // So user can reach _maxscore  
                //Framework.snd.play(Sound.Hallelujah);
                return;
            }
            
            

            // (1) End the game if the player is dead 
            if ((Health == 0) && !_gameover) //Player's been killed and game's not over.
            {
                _gameover = true;
               
                Player.addScore(_scorecorrection); // So user can reach _maxscore  
                //Framework.snd.play(Sound.Hallelujah);
                return; 
            }
            
            // (3) Maybe check some other conditions.

            if (wentThrough && (Age - startNewRoom) > 2.0f)
            {
                wentThrough = false;
            }

            if (doorcollision == true)
            {
                if (_currentroom == 0)
                {
                    setRoom1();
                    Framework.snd.play(Sound.Teleport);
                    doorcollision = false;
                }

                else if (_currentroom == 1)
                {
                    setRoom2();
                    Framework.snd.play(Sound.Teleport);
                    doorcollision = false;
                }

                else if (_currentroom == 2)
                {
                    setRoom3();
                    Framework.snd.play(Sound.Teleport);
                    doorcollision = false;
                }
            }
        }

        public void checkDeath(cSprite Sprite)
        {
            if (_gameover == true)
            {
                Sprite.setstate(State.FallForwardDie, 0, 170, StateType.Hold);
            }
        }
    }
}