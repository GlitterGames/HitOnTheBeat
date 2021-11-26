using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colisions 
{
    public Floor floor;
    public List<int> positions;
    public List<int> eliminados;
    bool moreThanTwo;
    bool notCinematic;
    bool sameFloor;
    GameManager g;
    public Colisions(Floor f, GameManager g, bool sameFloor)
    {
        this.floor = f;
        this.g = g;
        this.sameFloor = sameFloor;
        notCinematic = true;
        positions = new List<int>();
        eliminados = new List<int>();
    }
    public void PerformColisions()
    {
        //Por todos los jugadores pone la fuerza cinética como su fuerza para colisionar con los demás jugadores
        for (int i = 0; i < positions.Count; i++)
        {
            if (g.jugadores[positions[i]].fuerzaCinetica > 0) g.jugadores[positions[i]].SetFuerza(g.jugadores[positions[i]].fuerzaCinetica);
        }
        if (Powers()) return;
        if (positions.Count <= 2)
        {
            this.moreThanTwo = false;
            PerformTwoColision();
        }
        else
        {
            this.moreThanTwo = true;
            PerformMoreThanTwoColision();
        }
        //Por todos los jugadores pone la fuerza cinética como su fuerza para colisionar con los demás jugadores
        for (int i = 0; i < positions.Count; i++)
        {
            g.jugadores[positions[i]].SetFuerza(0);
        }
    }
    public void PerformTwoColision()
    {
        int pos0 = positions[0];
        int pos1 = positions[1];
        //Si los dos jugadores tienen la misma fuerza se echan uno para atrás
        if (g.jugadores[pos0].Fuerza == g.jugadores[pos1].Fuerza)
        {
            DoubleColisionSameForce();
        }
        else
        {
            DoubleColisionNotSameForce();
        }
    }
    public void PerformMoreThanTwoColision()
    {
        int equal = -1;
        List<int> fuerzas = new List<int>();
        int max = GetMaxFuerza(out equal, out fuerzas);
        PerformAllColision(max, equal, fuerzas);
    }
    public void EndPowers()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            int pos = positions[i];
            if (g.jugadores[pos].Power == PlayerController.Power_Up.ESCUDO)
            {
                g.jugadores[pos].StopCoroutinePowers();
                g.jugadores[pos].EndPowerUp();
            }
            else
            {
                g.jugadores[pos].SetFuerza(0);
            }
        }
    }
    public bool PerformPowersColision()
    {
        bool powers = false;
        for (int i = 0; i < positions.Count; i++)
        {
            int pos = positions[i];
            if (g.jugadores[pos].Power == PlayerController.Power_Up.ESCUDO)
            {
                powers = true;
            }
        }
        if (powers)
        {
            if (!moreThanTwo)
            {
                DoubleColisionSameForce();
            }
            else
            {
                int equal = -1;
                List<int> fuerzas = new List<int>();
                int max = GetMaxFuerza(out equal, out fuerzas);
                //Igual que una colision de más de dos pero los que tienen escudo tienen fuerza máxima
                equal = -1;
                for (int i = 0; i < positions.Count; i++)
                {
                    int pos = positions[i];
                    if (g.jugadores[pos].Power == PlayerController.Power_Up.ESCUDO)
                    {
                        fuerzas[pos] = max;
                    }
                }
                PerformAllColision(max, equal, fuerzas);
            }
            EndPowers();
        }
        return powers;
    }
    public bool Powers()
    {
        if (positions.Count > 2)
        {
            this.moreThanTwo = false;
        }
        else
        {
            this.moreThanTwo = true;
        }
        return PerformPowersColision();
    }
    public bool PerformSingleColision(PlayerController p, FloorDetectorType dir, int fuerza, int golpeador, bool moreThanTwo, bool notCinematic, bool sameFloor)
    {
        return p.EcharOne(dir, fuerza, moreThanTwo, notCinematic, sameFloor, golpeador);
    }
    public void DoubleColisionSameForce()
    {
        int pos0 = positions[0];
        int pos1 = positions[1];
        FloorDetectorType dir0 = g.jugadores[pos0].floorDir;
        FloorDetectorType dir1 = g.jugadores[pos1].floorDir;
        int fuerza = 1;
        bool echado = PerformSingleColision(g.jugadores[pos0], dir1, fuerza, pos1, moreThanTwo, notCinematic, sameFloor);
        if (echado) eliminados.Add(pos0);
        echado = PerformSingleColision(g.jugadores[pos1], dir0, fuerza, pos0, moreThanTwo, notCinematic, sameFloor);
        if (echado) eliminados.Add(pos1);
    }
    public void DoubleColisionNotSameForce()
    {
        int pos0 = positions[0];
        int pos1 = positions[1];
        if (g.jugadores[pos1].Fuerza > g.jugadores[pos0].Fuerza)
        {
            pos0 = positions[1];
            pos1 = positions[0];
        }
        g.jugadores[pos0].Golpear();
        int max = g.jugadores[pos0].Fuerza - g.jugadores[pos1].Fuerza;
        bool echado = g.jugadores[pos1].EcharOne(g.jugadores[pos0].floorDir, max, moreThanTwo, notCinematic, sameFloor, pos0);
        if (echado) eliminados.Add(pos1);
    }
    public int GetMaxFuerza(out int equal, out List<int> fuerzas)
    {
        equal = -1;
        fuerzas = new List<int>();
        int maxFuerza = 0;
        //Se recorren todos los jugadores viendo que fuerza es la mayor
        for (int i = 0; i < positions.Count; i++)
        {
            //Tu fuerza es equivalente a la mayor posible divida entre los que colisionas
            //redondeada hacia arriba
            int fuerza = (int)Mathf.Ceil(g.jugadores[positions[i]].Fuerza / (positions.Count - 1));
            //Si hay uno con mayor fuerza la sobreescribe y dice que no hay nadie con su misma fuerza
            if (fuerza > maxFuerza)
            {
                equal = i;
                maxFuerza = fuerza;
            }
            //Si hay uno con igual fuerza dice que hay nadie con su misma fuerza
            else if (fuerza == maxFuerza)
            {
                equal = -1;
            }
            //finalmente añado la fuerza con la que pego al array de fuerzas
            fuerzas.Add(fuerza);
        }
        return maxFuerza;
    }
    public void PerformAllColision(int maxFuerza, int equal, List<int> fuerzas)
    {
        //Se realizan las colisiones
        for (int i = 0; i < positions.Count; i++)
        {
            int pos = positions[i];
            //Si eres la mayor fuerza y nadie te iguala golpeas
            if (fuerzas[i] == maxFuerza && equal != -1)
            {
                g.jugadores[pos].Golpear();
            }
            //Si eres la mayor fuerza pero alguien te iguala te caes una para atrás en la dirección opuesta que llevabas
            else
            {
                FloorDetectorType dir = g.jugadores[pos].floorDir;
                int fuerza = 1;
                if (fuerzas[i] != maxFuerza) fuerza = maxFuerza - fuerzas[i];
                bool echado = PerformSingleColision(g.jugadores[pos], dir, fuerza, equal, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(pos);
            }
        }
    }

}
