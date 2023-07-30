<template>
  <el-input
    v-model="numberMask"
    ref="inpNumberMask"
    @blur="maskNumber"
    @focus="realNumber"
  ></el-input>
  <input type="number" v-model="numberMask" ref="inpNumber" hidden />
</template>

<script>
import { ElInput } from "element-plus";
import { onMounted, ref, defineComponent } from "vue";

export default defineComponent({
  extends: ElInput,
  emit: ["value"],
  props: {
    value: Number,
    separateDecimal: {
      type: String,
      default: ",",
    },
    separateThousand: {
      type: String,
      default: ".",
    },
    numberDecimal: {
      type: Number,
      default: 2,
    },
    prefix: {
      type: String,
      default: "",
    },
    suffixes: {
      type: String,
      default: "",
    },
  },
  setup(props, { emit }) {
    const inpNumber = ref(null);
    const inpNumberMask = ref(null);
    const numberMask = ref(props.value);
    const number = ref(props.value);

    onMounted(() => {
      if (!inpNumberMask.value?.ref?.classList.contains("text-end")) {
        inpNumberMask.value.ref.classList += " text-end";
      }

      numberMask.value = fakeNumber(number.value);
    });

    const realNumber = () => {
      numberMask.value = number.value;
    };

    const maskNumber = () => {
      if (inpNumber.value?.value == inpNumberMask.value?.ref?.value) {
        number.value = inpNumber.value.value;
        numberMask.value = fakeNumber(inpNumber.value.value);
        emit("value", emitRealNumber(numberMask.value));
      } else {
        numberMask.value = null;
        number.value = null;
      }
    };

    const emitRealNumber = (numberMask) => {
      return (
        numberMask
          .replaceAll(props.separateThousand, "")
          .replaceAll(props.separateDecimal, ".") - "0"
      );
    };

    const fakeNumber = (number) => {
      let fake = `${number}`;
      let thousand = ``;
      let decimal = ``;
      fake.split(".").forEach((value, index) => {
        if (index == 0) thousand = value;
        else decimal = value;
      });

      let increament = 0;
      if (!decimal) {
        for (let i = 0; i < props.numberDecimal; i++) {
          decimal += "0";
        }
      } else {
        if (decimal.length <= props.numberDecimal) {
          decimal =
            (decimal - "0") *
            Math.pow(10, props.numberDecimal - decimal.length);
        } else {
          let hasIncreament =
            decimal.slice(props.numberDecimal, props.numberDecimal + 1) - "0" >=
            5;
          decimal = decimal.slice(0, props.numberDecimal);
          if (hasIncreament) {
            increament = 1;
            for (let i = decimal.length - 1; i >= 0; i--) {
              let digit = decimal[i] - "0";
              digit = digit + increament;
              if (digit == 10) {
                increament = 1;
                digit = 0;
              } else {
                increament = 0;
              }
              decimal =
                decimal.slice(0, i) + (digit + "") + decimal.slice(i + 1);
            }
          }
        }
      }
      thousand = thousand - "0" + increament + "";
      thousand = thousand.split("").reverse().join("");
      thousand = thousand
        .match(/.{1,3}/g)
        .join(props.separateThousand)
        .split("")
        .reverse()
        .join("");

      if (props.numberDecimal === 0) {
        if (thousand === "0") return "";
        return `${props.prefix}${thousand}${props.suffixes}`;
      }
      return `${props.prefix}${thousand}${props.separateDecimal}${decimal}${props.suffixes}`;
    };

    return {
      inpNumber,
      inpNumberMask,
      numberMask,
      maskNumber,
      realNumber,
      fakeNumber,
    };
  },
});
</script>