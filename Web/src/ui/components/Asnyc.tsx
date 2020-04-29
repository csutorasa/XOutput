import React from "react";
import CircularProgress from '@material-ui/core/CircularProgress';
import Typography from '@material-ui/core/Typography';
import { StyleGenerator, Styled } from "../../utils";
import { withStyles } from "@material-ui/core";


type ClassNames = 'centered';

const styles: StyleGenerator<ClassNames> = () => ({
  centered: {
      textAlign: 'center',
  },
});

export interface AsyncComponentProps<T> extends Styled<ClassNames> {
    task: Promise<T>;
    render: (data?: T) => any;
    size?: number | string;
}

interface AsyncComponentState<T> {
    loaded: boolean;
    success?: boolean;
    result?: T;
    error?: string;
}

export function AsyncErrorHandler(component: React.Component): (err: any) => void {
    return (err: any) => {
        component.forceUpdate();
        throw err;
    }
}

class AsyncComponent<T> extends React.Component<AsyncComponentProps<T>, AsyncComponentState<T>> {

    state: AsyncComponentState<T> = {
        loaded: false,
    }
    private subscribed: boolean = false;

    componentDidMount() {
        if (this.props.task) {
            this.subscribe();
        }
    }

    componentDidUpdate() {
        if (!this.subscribed && this.props.task) {
            this.subscribe();
        }
    }

    private subscribe(): void {
        this.props.task.then((data) => {
            this.setState({
                loaded: true,
                success: true,
                result: data,
                error: null,
            })
        }, (err: Error) => {
            this.setState({
                loaded: true,
                success: false,
                result: null,
                error: err.toString(),
            })
            console.log('' + err)
        });
        this.subscribed = true;
    }

    render() {
        const { classes } = this.props;
        if (this.state.loaded === false) {
            return <div className={classes.centered}><CircularProgress size={this.props.size || '10rem'} /></div>
        }
        if (this.state.success === false) {
            return <Typography color='error'>{this.state.error}</Typography>
        }
        return <>
            {this.props.render(this.state.result)}
        </>;
    }
}

export const Async = withStyles(styles)(AsyncComponent);
export default Async;
