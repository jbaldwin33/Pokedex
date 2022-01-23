using System;
using System.Collections.Generic;
using System.Linq;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexLib
{
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
        public TypeEnum ThisType { get; }
        public List<TypeMult> Resistances { get; } = new List<TypeMult>();
        public List<TypeMult> Weaknesses { get; } = new List<TypeMult>();
        public List<TypeMult> Immunities { get; } = new List<TypeMult>();
        public List<TypeMult> NormalDamage { get; } = new List<TypeMult>();

        public TypeClass(TypeEnum type, List<TypeEnum> res, List<TypeEnum> weak, List<TypeEnum> imm)
        {
            ThisType = type;
            var exclude = res.Concat(weak).Concat(imm);
            var norms = Enum.GetValues(typeof(TypeEnum)).Cast<TypeEnum>().Except(exclude).ToList();

            res.ForEach(r => Resistances.Add(new TypeMult(r, 0.5)));
            weak.ForEach(w => Weaknesses.Add(new TypeMult(w, 2)));
            imm.ForEach(i => Immunities.Add(new TypeMult(i, 0)));
            norms.ForEach(n => NormalDamage.Add(new TypeMult(n, 1)));
        }
    }

    public class DualTypeClass
    {
        public TypeClass Type1 { get; }
        public TypeClass Type2 { get; }
        public List<TypeMult> Resistances { get; } = new List<TypeMult>();
        public List<TypeMult> Weaknesses { get; } = new List<TypeMult>();
        public List<TypeMult> Immunities { get; } = new List<TypeMult>();
        public List<TypeMult> NormalDamage { get; } = new List<TypeMult>();

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
        private static readonly Lazy<TypeMasterClass> lazy = new(() => new TypeMasterClass());
        public static TypeMasterClass Instance => lazy.Value;

        private TypeClass type1;
        private TypeClass type2;
        private List<TypeMult> resistancesChecked;
        private List<TypeMult> weaknessesChecked;
        private DualTypeClass dualTypeClass;
        private List<DualTypeClass> dualTypeCombos;
        private TypeClass[] typeClasses;
        public List<DualTypeClass> DualTypeCombos => dualTypeCombos ??= GetDualTypeCombos();
        public TypeClass[] TypeClasses => typeClasses ??= CreateTypes();

        public TypeMasterClass()
        {
            typeClasses = CreateTypes();
        }

        private List<DualTypeClass> GetDualTypeCombos()
        {
            var dualTypeClasses = new List<DualTypeClass>();

            for (var i = 0; i < TypeClasses.Length - 1; i++)
            {
                for (var j = 1; j < TypeClasses.Length; j++)
                {
                    type1 = TypeClasses[i];
                    type2 = TypeClasses[j];
                    if (type1.ThisType == type2.ThisType)
                        continue;

                    resistancesChecked = new List<TypeMult>();
                    weaknessesChecked = new List<TypeMult>();
                    dualTypeClass = new DualTypeClass(type1, type2);
                    CheckResistances();
                    CheckWeaknesses();
                    SetNormalDamage();

                    dualTypeClasses.Add(dualTypeClass);
                }
            }
            return dualTypeClasses;
        }

        private void CheckResistances()
        {
            foreach (var res in type1.Resistances)
            {
                if (type2.Resistances.Any(x => x.ThisType == res.ThisType))
                {
                    dualTypeClass.Resistances.Add(new TypeMult(res.ThisType, 0.25));
                    resistancesChecked.Add(res);
                }
                else if (type2.Weaknesses.Any(x => x.ThisType == res.ThisType))
                    weaknessesChecked.Add(res);
                else if (!type2.Immunities.Contains(res))
                    dualTypeClass.Resistances.Add(res);
            }

            foreach (var res in type2.Resistances)
            {
                if (resistancesChecked.Any(x => x.ThisType == res.ThisType) || weaknessesChecked.Any(x => x.ThisType == res.ThisType))
                    continue;

                if (type1.Weaknesses.Any(x => x.ThisType == res.ThisType))
                    weaknessesChecked.Add(res);
                else if (!type1.Immunities.Any(x => x.ThisType == res.ThisType))
                    dualTypeClass.Resistances.Add(res);
            }
        }

        private void CheckWeaknesses()
        {
            foreach (var weak in type1.Weaknesses)
            {
                if (resistancesChecked.Any(x => x.ThisType == weak.ThisType) || weaknessesChecked.Any(x => x.ThisType == weak.ThisType))
                    continue;

                if (type2.Weaknesses.Any(x => x.ThisType == weak.ThisType))
                {
                    dualTypeClass.Weaknesses.Add(new TypeMult(weak.ThisType, 4));
                    weaknessesChecked.Add(weak);
                }
                else if (!type2.Immunities.Any(x => x.ThisType == weak.ThisType))
                    dualTypeClass.Weaknesses.Add(weak);
            }

            foreach (var weak in type2.Weaknesses)
            {
                if (resistancesChecked.Any(x => x.ThisType == weak.ThisType) || weaknessesChecked.Any(x => x.ThisType == weak.ThisType))
                    continue;

                if (!type1.Immunities.Any(x => x.ThisType == weak.ThisType))
                    dualTypeClass.Weaknesses.Add(weak);
            }
        }

        private void SetNormalDamage()
        {
            var exclude = dualTypeClass.Resistances.Select(x => x.ThisType).Concat(dualTypeClass.Weaknesses.Select(x => x.ThisType)).Concat(dualTypeClass.Immunities.Select(x => x.ThisType));
            var norms = Enum.GetValues(typeof(TypeEnum)).Cast<TypeEnum>().ToList().Except(exclude);
            foreach (var n in norms)
                dualTypeClass.NormalDamage.Add(new TypeMult(n, 1));
        }

        private static TypeClass[] CreateTypes() =>
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
