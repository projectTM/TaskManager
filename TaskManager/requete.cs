using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    public class requete
    {
        /**************************************************************/
        /*                       Requete d'accès                      */
        /**************************************************************/

        //public System.Linq.IQueryable getTaches(mediametrieEntities bdd, string nomContainer)
        //{
        //    var list = (from t in bdd.taches
        //                where (t.label_container == nomContainer)
        //                select t);
        //    return list;
        //}

        public List<taches> getTachesContainer(mediametrieEntities bdd, string nomContainer)
        {
            List<taches> list = (from t in bdd.taches1
                                 where (t.label_container == nomContainer)
                                 select t).ToList();
            return list;
        }

        public List<taches> getTaches(mediametrieEntities bdd)
        {
            List<taches> list = (from t in bdd.taches1
                                 select t).ToList();
            return list;
        }

        public taches getSTaches(mediametrieEntities bdd, string label)
        {
            taches list = (from t in bdd.taches1
                           where (t.label_tache == label)
                           select t).FirstOrDefault();
            return list;
        }

        public List<taches> getSousTaches(mediametrieEntities bdd, string nomTache)
        {
            List<taches> list = (from t in bdd.taches1
                                 where (t.label_tache_parent == nomTache)
                                 select t).ToList();
            return list;
        }

        public List<container> getContainer(mediametrieEntities bdd)
        {
            List<container> list = (from t in bdd.containers
                                    select t).ToList();
            return list;
        }

        public container getSContainer(mediametrieEntities bdd, string label)
        {
            container c = (from t in bdd.containers
                           where (t.label == label)
                           select t).FirstOrDefault();
            return c;
        }

        public int getNbTacheContainer(mediametrieEntities bdd, string nomContainer)
        {
            int nbTache = (from t in bdd.taches1
                           where t.label_container == nomContainer
                           select t).Count();
            return nbTache;
        }


        /**************************************************************/
        /*                       Requete Ajout                        */
        /**************************************************************/

        public void ajoutTaches(mediametrieEntities bdd, taches laTaches)
        {
            bdd.taches1.Add(laTaches);
            bdd.SaveChanges();
        }

        public void ajoutContainer(mediametrieEntities bdd, container leContainer)
        {
            bdd.containers.Add(leContainer);
            bdd.SaveChanges();
        }

        /**************************************************************/
        /*                    Requete Modification                    */
        /**************************************************************/

        public void modifTaches(mediametrieEntities bdd, taches laTaches)
        {
            /* Ajouter le changement  */
            taches original = bdd.taches1.Find(laTaches.label_tache);
            if (original != null)
            {
                //bdd.Entry(original).CurrentValues.SetValues(laTaches);
                original.label_container = laTaches.label_container;
                //original.label_tache = laTaches.label_tache;
                original.label_tache_parent = laTaches.label_tache_parent;
                original.date_debut = laTaches.date_debut;
                original.date_fin = laTaches.date_fin;
                original.commentaire = laTaches.commentaire;
                original.effectuer = laTaches.effectuer;
                bdd.SaveChanges();
            }
        }

        public void modifContainer(mediametrieEntities bdd, container leContainer)
        {
            /* Ajouter le changement  */
            container original = bdd.containers.Find(leContainer.label);
            if (original != null)
            {
                bdd.Entry(original).CurrentValues.SetValues(leContainer);
                bdd.SaveChanges();
            }
        }

        /**************************************************************/
        /*                      Requete Suppression                   */
        /**************************************************************/

        public void supTaches(mediametrieEntities bdd, taches laTaches)
        {
            //taches s = getSTaches(bdd, laTaches.label_tache);
            if (laTaches != null)
            {
                bdd.taches1.Remove(laTaches);
                bdd.SaveChanges();
            }
        }

        public void supContainer(mediametrieEntities bdd, container leContainer)
        {
            List<taches> l = (from t in bdd.taches1
                              where t.label_container == leContainer.label
                              select t).ToList();
            foreach (taches e in l)
            {
                supTaches(bdd, e);
            }
            bdd.containers.Remove(leContainer);
            bdd.SaveChanges();
        }
    }
}
