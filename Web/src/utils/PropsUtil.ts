import { Theme } from "@material-ui/core/styles";
import { Styles } from "@material-ui/core/styles/withStyles";

export type StyleGenerator<T extends string> = Styles<Theme, any, T>;

export type Classes<T extends string> = { [className in T]: string };

export interface Styled<T extends string> {
    classes?: Classes<T>;
}

