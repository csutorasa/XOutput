import { Theme } from "@material-ui/core/styles";
import { Styles } from "@material-ui/core/styles/withStyles";

export type StyleGenerator<T extends string> = Styles<Theme, any, T>;

export interface Styled<T extends string> {
    classes?: {[className in T]: string }
}

