import { Theme } from '@mui/material/styles';
import { Styles } from '@mui/styles/withStyles';

export type StyleGenerator<T extends string, P extends object = object> = Styles<Theme, P, T>;

export type Classes<T extends string> = { [className in T]: string };

export interface Styled<T extends string> {
  classes: Classes<T>;
}
