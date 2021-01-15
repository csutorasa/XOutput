import React from "react";
import green from "@material-ui/core/colors/green";
import { HashRouter } from "react-router-dom";
import { MainMenu } from "./mainmenu";
import { ThemeProvider, createMuiTheme } from "@material-ui/core";

const productionTheme = createMuiTheme({

});

const developmentTheme = createMuiTheme({
    palette: {
        primary: green,
    }
});

const theme = process.env.NODE_ENV !== 'production' ? productionTheme : productionTheme;

export const RootElement = <HashRouter>
    <ThemeProvider theme={theme}>
        <MainMenu></MainMenu>
    </ThemeProvider>
</HashRouter>;
export default RootElement;
