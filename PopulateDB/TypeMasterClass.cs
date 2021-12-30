using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.PokedexLib
{
    public enum TypeEnum
    {
        Normal,
        Fire,
        Water,
        Grass,
        Electric,
        Ice,
        Fighting,
        Poison,
        Ground,
        Flying,
        Psychic,
        Bug,
        Rock,
        Ghost,
        Dark,
        Dragon,
        Steel,
        Fairy
    }

    public class TypeMult
    {
        public TypeEnum ThisType { get; set; }
        public double Multiplier { get; set; }
        public TypeMult(TypeEnum type, double mult)
        {
            ThisType = type;
            Multiplier = mult;
        }
    }

    public class TypeClass
    {
        public TypeEnum ThisType;
        public List<TypeMult> Resistances = new List<TypeMult>();
        public List<TypeMult> Weaknesses = new List<TypeMult>();
        public List<TypeMult> Immunities = new List<TypeMult>();
        public List<TypeMult> NormalDamage = new List<TypeMult>();

        public TypeClass(TypeEnum type, List<TypeEnum> res, List<TypeEnum> weak, List<TypeEnum> imm)
        {
            ThisType = type;
            var exclude = Resistances.Select(x => x.ThisType).Concat(Weaknesses.Select(x => x.ThisType)).Concat(Immunities.Select(x => x.ThisType));
            var norms = Enum.GetValues(typeof(TypeEnum)).Cast<TypeEnum>().ToList().Except(exclude);

            foreach (var r in res)
                Resistances.Add(new TypeMult(r, 0.5));
            foreach (var w in weak)
                Weaknesses.Add(new TypeMult(w, 2));
            foreach (var i in imm)
                Immunities.Add(new TypeMult(i, 0));
            foreach (var n in norms)
                NormalDamage.Add(new TypeMult(n, 1));
        }
    }

    public class DualTypeClass
    {
        public TypeClass Type1;
        public TypeClass Type2;
        public List<TypeMult> Resistances = new List<TypeMult>();
        public List<TypeMult> Weaknesses = new List<TypeMult>();
        public List<TypeMult> Immunities = new List<TypeMult>();
        public List<TypeMult> NormalDamage = new List<TypeMult>();

        public DualTypeClass(TypeClass type1, TypeClass type2)
        {
            Type1 = type1;
            Type2 = type2;
            type1.Immunities.ForEach(Immunities.Add);
            type2.Immunities.ForEach(Immunities.Add);
        }
    }

    public class TypeMasterClass
    {
        private static readonly Lazy<TypeMasterClass> lazy = new Lazy<TypeMasterClass>(() => new TypeMasterClass());
        public static TypeMasterClass Instance => lazy.Value;
        private TypeMasterClass()
        {
            TypeClasses = CreateTypes();
        }


        public TypeClass[] TypeClasses { get; set; }

        public TypeClass[] CreateTypes() =>
            new TypeClass[]
            {
                new TypeClass(TypeEnum.Normal, new List<TypeEnum>(), new List<TypeEnum>{ TypeEnum.Fighting }, new List<TypeEnum> { TypeEnum.Ghost }),
                new TypeClass(TypeEnum.Fire, new List<TypeEnum> { TypeEnum.Fire, TypeEnum.Grass, TypeEnum.Ice, TypeEnum.Bug, TypeEnum.Steel, TypeEnum.Fairy }, new List<TypeEnum> { TypeEnum.Water, TypeEnum.Ground, TypeEnum.Rock }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Water, new List<TypeEnum> { TypeEnum.Fire, TypeEnum.Water, TypeEnum.Ice, TypeEnum.Steel }, new List<TypeEnum> {TypeEnum.Electric, TypeEnum.Grass }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Grass, new List<TypeEnum> { TypeEnum.Water, TypeEnum.Electric, TypeEnum.Grass, TypeEnum.Ground }, new List<TypeEnum> {TypeEnum.Fire, TypeEnum.Ice, TypeEnum.Poison, TypeEnum.Flying, TypeEnum.Bug }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Electric, new List<TypeEnum> { TypeEnum.Electric, TypeEnum.Flying, TypeEnum.Steel }, new List<TypeEnum> {TypeEnum.Ground }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Ice, new List<TypeEnum> { TypeEnum.Ice }, new List<TypeEnum> {TypeEnum.Fire, TypeEnum.Fighting, TypeEnum.Rock, TypeEnum.Steel }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Fighting, new List<TypeEnum> { TypeEnum.Bug, TypeEnum.Rock, TypeEnum.Dark }, new List<TypeEnum> {TypeEnum.Flying, TypeEnum.Psychic, TypeEnum.Fairy }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Poison, new List<TypeEnum> { TypeEnum.Grass, TypeEnum.Fighting, TypeEnum.Poison, TypeEnum.Bug, TypeEnum.Fairy }, new List<TypeEnum> {TypeEnum.Ground, TypeEnum.Psychic }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Ground, new List<TypeEnum> { TypeEnum.Poison, TypeEnum.Rock }, new List<TypeEnum> {TypeEnum.Water, TypeEnum.Grass, TypeEnum.Ice }, new List<TypeEnum> { TypeEnum.Electric }),
                new TypeClass(TypeEnum.Flying, new List<TypeEnum> { TypeEnum.Grass, TypeEnum.Fighting, TypeEnum.Bug }, new List<TypeEnum> {TypeEnum.Electric, TypeEnum.Ice, TypeEnum.Rock }, new List<TypeEnum> { TypeEnum.Ground }),
                new TypeClass(TypeEnum.Psychic, new List<TypeEnum> { TypeEnum.Fighting, TypeEnum.Psychic }, new List<TypeEnum> { TypeEnum.Bug, TypeEnum.Ghost, TypeEnum.Dark }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Bug, new List<TypeEnum> { TypeEnum.Grass, TypeEnum.Fighting, TypeEnum.Ground }, new List<TypeEnum> {TypeEnum.Fire, TypeEnum.Flying, TypeEnum.Rock }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Rock, new List<TypeEnum> { TypeEnum.Normal, TypeEnum.Fire, TypeEnum.Poison, TypeEnum.Flying }, new List<TypeEnum> {TypeEnum.Water, TypeEnum.Grass, TypeEnum.Fighting, TypeEnum.Ground, TypeEnum.Steel }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Ghost, new List<TypeEnum> { TypeEnum.Poison, TypeEnum.Bug }, new List<TypeEnum> {TypeEnum.Ghost, TypeEnum.Dark }, new List<TypeEnum> { TypeEnum.Normal, TypeEnum.Fighting }),
                new TypeClass(TypeEnum.Dark, new List<TypeEnum> { TypeEnum.Ghost, TypeEnum.Dark }, new List<TypeEnum> {TypeEnum.Fighting, TypeEnum.Bug, TypeEnum.Fairy }, new List<TypeEnum> { TypeEnum.Psychic }),
                new TypeClass(TypeEnum.Dragon, new List<TypeEnum> { TypeEnum.Fire, TypeEnum.Water, TypeEnum.Electric, TypeEnum.Grass }, new List<TypeEnum> {TypeEnum.Ice, TypeEnum.Dragon, TypeEnum.Fairy }, new List<TypeEnum>()),
                new TypeClass(TypeEnum.Steel, new List<TypeEnum> { TypeEnum.Normal, TypeEnum.Grass, TypeEnum.Ice, TypeEnum.Flying, TypeEnum.Psychic, TypeEnum.Bug, TypeEnum.Rock, TypeEnum.Dragon, TypeEnum.Steel, TypeEnum.Fairy }, new List<TypeEnum> {TypeEnum.Fire, TypeEnum.Fighting, TypeEnum.Ground }, new List<TypeEnum> { TypeEnum.Poison }),
                new TypeClass(TypeEnum.Fairy, new List<TypeEnum> { TypeEnum.Fighting, TypeEnum.Bug, TypeEnum.Dark }, new List<TypeEnum> {TypeEnum.Poison, TypeEnum.Steel }, new List<TypeEnum> { TypeEnum.Dragon })
            };
    }
}
