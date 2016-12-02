using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    class requete
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
            List<taches> list = (from t in bdd.taches
                                 where (t.label_container == nomContainer)
                                 select t).ToList();
            return list;
        }

        public List<taches> getTaches(mediametrieEntities bdd)
        {
            List<taches> list = (from t in bdd.taches
                                 select t).ToList();
            return list;
        }

        public List<container> getContainer(mediametrieEntities bdd)
        {
            List<container> list = (from t in bdd.container
                                    select t).ToList();
            return list;
        }

        public int getNbTacheContainer(mediametrieEntities bdd, string nomContainer)
        {
            int nbTache = (from t in bdd.taches
                           where t.label_container == nomContainer
                           select t).Count();
            return nbTache;
        }


        /**************************************************************/
        /*                       Requete Ajout                        */
        /**************************************************************/

        public void ajoutTaches(mediametrieEntities bdd, taches laTaches)
        {
            bdd.taches.Add(laTaches);
            bdd.SaveChanges();
        }

        public void ajoutContainer(mediametrieEntities bdd, container leContainer)
        {
            bdd.container.Add(leContainer);
            bdd.SaveChanges();
        }

        /**************************************************************/
        /*                    Requete Modification                    */
        /**************************************************************/

        public void modifTaches(mediametrieEntities bdd, taches laTaches)
        {
            /* Ajouter le changement  */
            bdd.SaveChanges();
        }

        public void modifContainer(mediametrieEntities bdd, container leContainer)
        {
            /* Ajouter le changement  */
            bdd.SaveChanges();
        }

        /**************************************************************/
        /*                      Requete Suppression                   */
        /**************************************************************/

        public void supTaches(mediametrieEntities bdd, taches laTaches)
        {
            bdd.taches.Remove(laTaches);
            bdd.SaveChanges();
        }

        public void supContainer(mediametrieEntities bdd, container leContainer)
        {
            bdd.container.Remove(leContainer);
            bdd.SaveChanges();
        }
    }
}
