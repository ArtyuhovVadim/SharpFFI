using System.Runtime.CompilerServices;

namespace SharpFFI.Primitives.Tests;

public class Bool32Tests
{
    [Fact]
    public void Ctor_FromBool_SetsZeroOrOne()
    {
        // Arrange
        const bool falseInput = false;
        const bool trueInput = true;

        // Act
        var b32False = new Bool32(falseInput);
        var b32True = new Bool32(trueInput);

        // Assert
        b32False.Value.ShouldBe(0u);
        b32True.Value.ShouldBe(1u);
    }

    [Fact]
    public void Ctor_FromInteger_NormalizesNonZeroToOne()
    {
        // Arrange
        uint[] testUints = [0u, 1u, 42u, (uint)int.MaxValue, uint.MaxValue];

        // Act & Assert
        foreach (var u in testUints)
        {
            var bool32 = new Bool32(u);
            var expected = u == 0u ? 0u : 1u;
            bool32.Value.ShouldBe(expected);
        }
    }

    [Fact]
    public void Ctor_CrossConversion_PreservesTruthiness()
    {
        // Arrange
        var b8False = new Bool8(false);
        var b8True = new Bool8(true);

        // Act
        var b32From8False = new Bool32(b8False);
        var b32From8True = new Bool32(b8True);

        // Assert
        b32From8False.Value.ShouldBe(0u);
        b32From8True.Value.ShouldBe(1u);
    }

    [Fact]
    public void UnnormalizedMemory_BehavesAsCanonicalTrue()
    {
        // Arrange
        var raw32 = 100u;
        ref var b32Unnormalized = ref Unsafe.As<uint, Bool32>(ref raw32);
        var b32Canonical = new Bool32(true);

        var raw32_2 = 2u;
        ref var b32Unnormalized2 = ref Unsafe.As<uint, Bool32>(ref raw32_2);

        // Act & Assert
        b32Unnormalized.Value.ShouldBe(100u);
        b32Unnormalized.ShouldBe(b32Canonical);
        b32Unnormalized2.ShouldBe(b32Canonical);
        b32Unnormalized.GetHashCode().ShouldBe(1);
        b32Unnormalized.CompareTo(b32Canonical).ShouldBe(0);
        b32Unnormalized.ToString().ShouldBe("True");
    }

    [Fact]
    public unsafe void MemoryLayout_SizeOf_MatchesAbi()
    {
        // Arrange & Act
        var size = sizeof(Bool32);

        // Assert
        size.ShouldBe(4);
    }

    [Fact]
    public void Equality_AllCombinations_ReturnsExpected()
    {
        // Arrange
        var b32F1 = new Bool32(false);
        var b32F2 = new Bool32(false);
        var b32T1 = new Bool32(true);
        var b32T2 = new Bool32(true);

        // Act & Assert
        b32F1.ShouldBe(b32F2);
        b32F1.Equals((object)b32F2).ShouldBeTrue();

        b32F1.ShouldNotBe(b32T1);
        b32F1.Equals((object)b32T1).ShouldBeFalse();

        b32T1.ShouldNotBe(b32F1);
        b32T1.Equals((object)b32F1).ShouldBeFalse();

        b32T1.ShouldBe(b32T2);
        b32T1.Equals((object)b32T2).ShouldBeTrue();

        b32T1.Equals(null).ShouldBeFalse();
        b32T1.Equals("wrong_type").ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_ReturnsZeroOrOne()
    {
        // Arrange
        var b32False = new Bool32(false);
        var b32True = new Bool32(true);

        // Act
        var hashFalse = b32False.GetHashCode();
        var hashTrue = b32True.GetHashCode();

        // Assert
        hashFalse.ShouldBe(0);
        hashTrue.ShouldBe(1);
    }

    [Fact]
    public void Comparison_OperatorsAndCompareTo_FollowBooleanOrder()
    {
        // Arrange
        var b32F1 = new Bool32(false);
        var b32F2 = new Bool32(false);
        var b32T1 = new Bool32(true);
        var b32T2 = new Bool32(true);

        // Act & Assert
        b32F1.ShouldBeLessThan(b32T1);
        b32F1.ShouldBeLessThanOrEqualTo(b32T1);
        b32T1.ShouldBeGreaterThan(b32F1);
        b32T1.ShouldBeGreaterThanOrEqualTo(b32F1);

        b32F1.ShouldBeLessThanOrEqualTo(b32F2);
        b32F1.ShouldBeGreaterThanOrEqualTo(b32F2);
        b32F1.CompareTo(b32F2).ShouldBe(0);

        b32T1.ShouldBeLessThanOrEqualTo(b32T2);
        b32T1.ShouldBeGreaterThanOrEqualTo(b32T2);
        b32T1.CompareTo(b32T2).ShouldBe(0);

        b32F1.CompareTo(b32T1).ShouldBeNegative();
        b32T1.CompareTo(b32F1).ShouldBePositive();
    }

    [Fact]
    public void CompareTo_Object_HandlesNullAndInvalidType()
    {
        // Arrange
        var b32 = new Bool32(true);

        // Act
        var nullCompare = b32.CompareTo((object?)null);

        // Assert
        nullCompare.ShouldBe(1);
        Should.Throw<ArgumentException>(() => b32.CompareTo(12345));
    }

    [Fact]
    public void ImplicitConversions_BoolAndCrossType_Roundtrip()
    {
        // Arrange
        var managedTrue = true;
        var managedFalse = false;

        // Act
        Bool32 b32FromTrue = managedTrue;
        bool boolFromB32True = b32FromTrue;

        Bool32 b32FromFalse = managedFalse;
        bool boolFromB32False = b32FromFalse;

        Bool8 b8 = b32FromTrue;
        Bool32 b32FromB8 = b8;

        // Assert
        b32FromTrue.Value.ShouldBe(1u);
        boolFromB32True.ShouldBeTrue();

        b32FromFalse.Value.ShouldBe(0u);
        boolFromB32False.ShouldBeFalse();

        b8.Value.ShouldBe((byte)1);
        b32FromB8.Value.ShouldBe(1u);
    }

    [Fact]
    public void LogicalOperators_NotTrueFalse_WorkInShortCircuit()
    {
        // Arrange
        var b32True = new Bool32(true);
        var b32False = new Bool32(false);

        // Act & Assert - Unary !
        (!b32True).ShouldBeFalse();
        (!b32False).ShouldBeTrue();

        // Act & Assert - If branches
        var ifTrueExecuted = false;
        if (b32True)
            ifTrueExecuted = true;
        ifTrueExecuted.ShouldBeTrue();

        var ifFalseExecuted = false;
        if (b32False)
            ifFalseExecuted = true;
        ifFalseExecuted.ShouldBeFalse();

        // Act & Assert - Short-circuiting
        var sideEffectCalled = false;

        if (b32False && SideEffect())
        {
        }
        sideEffectCalled.ShouldBeFalse();

        if (b32True || SideEffect())
        {
        }
        sideEffectCalled.ShouldBeFalse();

        return;

        bool SideEffect()
        {
            sideEffectCalled = true;
            return true;
        }
    }

    [Fact]
    public void EqualityOperators_DirectInvocation_ReturnsExpected()
    {
        // Arrange
        var b32False1 = new Bool32(false);
        var b32False2 = new Bool32(false);
        var b32True1 = new Bool32(true);
        var b32True2 = new Bool32(true);

        // Act
        var eqSameFalse = b32False1 == b32False2;
        var eqSameTrue = b32True1 == b32True2;
        var eqDiff = b32False1 == b32True1;
        var neqDiff = b32False1 != b32True1;
        var neqSame = b32False1 != b32False2;

        // Assert
        eqSameFalse.ShouldBeTrue();
        eqSameTrue.ShouldBeTrue();
        eqDiff.ShouldBeFalse();
        neqDiff.ShouldBeTrue();
        neqSame.ShouldBeFalse();
    }

    [Fact]
    public void RelationalOperators_DirectInvocation_ReturnsExpected()
    {
        // Arrange
        var b32False = new Bool32(false);
        var b32True = new Bool32(true);

        // Act
        var lt = b32False < b32True;
        var lteSame = b32False <= b32False;
        var lteDiff = b32False <= b32True;
        var gt = b32True > b32False;
        var gteSame = b32True >= b32True;
        var gteDiff = b32True >= b32False;

        var notLt = b32True < b32False;
        var notGt = b32False > b32True;

        // Assert
        lt.ShouldBeTrue();
        lteSame.ShouldBeTrue();
        lteDiff.ShouldBeTrue();
        gt.ShouldBeTrue();
        gteSame.ShouldBeTrue();
        gteDiff.ShouldBeTrue();
        notLt.ShouldBeFalse();
        notGt.ShouldBeFalse();
    }

    [Fact]
    public void UserDefinedTrueFalseOperators_ViaReflection_ReturnExpected()
    {
        // Arrange
        var b32False = new Bool32(false);
        var b32True = new Bool32(true);
        var opTrue = typeof(Bool32).GetMethod("op_True", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!;
        var opFalse = typeof(Bool32).GetMethod("op_False", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!;

        // Act
        var trueOnTrue = (bool)opTrue.Invoke(null, [b32True])!;
        var trueOnFalse = (bool)opTrue.Invoke(null, [b32False])!;
        var falseOnFalse = (bool)opFalse.Invoke(null, [b32False])!;
        var falseOnTrue = (bool)opFalse.Invoke(null, [b32True])!;

        // Assert
        trueOnTrue.ShouldBeTrue();
        trueOnFalse.ShouldBeFalse();
        falseOnFalse.ShouldBeTrue();
        falseOnTrue.ShouldBeFalse();
    }
}
